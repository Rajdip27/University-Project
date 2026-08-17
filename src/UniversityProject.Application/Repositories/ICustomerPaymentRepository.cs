using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UniversityProject.Application.ViewModel;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ICustomerPaymentRepository
{
    Task<List<CustomerUnpaidInvoiceViewModel>> GetCustomerUnpaidInvoices(long customerId);
    Task<SupplierLedgerReportDto> GetSupplierLedgerReport(long? supplierId,DateTime? startDate, DateTime? endDate);
    Task<bool> SaveAsync(
        CustomerPaymentViewModel model,
        long userId);
    Task<(List<CustomerPaymentListViewModel> Items, int TotalCount)> GetListAsync(
    string search,
    int pageNo,
    int pageSize);
    Task<( CustomerLedgerReportSummaryViewModel Summary,List<CustomerLedgerReportViewModel> Items)> GetLedgerReportAsync(long? customerId, DateTime? startDate, DateTime? endDate);
}

public class CustomerPaymentRepository : ICustomerPaymentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerPaymentRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SupplierLedgerReportDto> GetSupplierLedgerReport(
    long? supplierId,
    DateTime? startDate,
    DateTime? endDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_SupplierLedgerReport",
            new
            {
                SupplierId = supplierId,
                StartDate = startDate,
                EndDate = endDate
            },
            commandType: CommandType.StoredProcedure
        );

        var summary = await multi.ReadFirstOrDefaultAsync<SupplierLedgerSummaryDto>();

        var transactions = (await multi.ReadAsync<SupplierLedgerTransactionDto>())
            .ToList();

        return new SupplierLedgerReportDto
        {
            SupplierId = supplierId,
            StartDate = startDate,
            EndDate = endDate,

            Summary = summary ?? new SupplierLedgerSummaryDto(),

            Transactions = transactions
        };
    }
    public async Task<(
    CustomerLedgerReportSummaryViewModel Summary,List<CustomerLedgerReportViewModel> Items)> GetLedgerReportAsync(long? customerId,DateTime? startDate,DateTime? endDate)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add(
            "@CustomerId",
            customerId,
            DbType.Int64);

        parameters.Add(
            "@StartDate",
            startDate,
            DbType.Date);

        parameters.Add(
            "@EndDate",
            endDate,
            DbType.Date);

        using var multi = await connection.QueryMultipleAsync(
            "sp_CustomerLedger_Report",
            parameters,
            commandType: CommandType.StoredProcedure);

        // Result Set 1 - Summary
        var summary =
            await multi.ReadFirstAsync<CustomerLedgerReportSummaryViewModel>();

        // Result Set 2 - Ledger Transactions
        var items =
            (await multi.ReadAsync<CustomerLedgerReportViewModel>())
            .ToList();

        return (summary, items);
    }
    public async Task<(List<CustomerPaymentListViewModel> Items, int TotalCount)> GetListAsync(
    string search,
    int pageNo,
    int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@Search", search);
        parameters.Add("@PageNo", pageNo);
        parameters.Add("@PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_CustomerPayment_List",
            parameters,
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<CustomerPaymentListViewModel>())
            .ToList();

        var totalCount = await multi.ReadFirstAsync<int>();

        return (items, totalCount);
    }
    public async Task<List<CustomerUnpaidInvoiceViewModel>>
        GetCustomerUnpaidInvoices(long customerId)
    {
        using var connection =
            _connectionFactory.CreateConnection();
        var result =
            await connection.QueryAsync<CustomerUnpaidInvoiceViewModel>(
                "sp_Customer_UnpaidInvoices",
                new
                {
                    CustomerId = customerId
                },
                commandType: CommandType.StoredProcedure);

        return result.ToList();
    }


    //====================================================
    // SAVE CUSTOMER PAYMENT
    //====================================================

    public async Task<bool> SaveAsync(
        CustomerPaymentViewModel model,
        long userId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction =
            connection.BeginTransaction();

        try
        {
            foreach (var invoice in model.Invoices
                         .Where(x => x.CollectionAmount > 0))
            {
                var parameters = new DynamicParameters();

                parameters.Add(
                    "@CustomerPaymentId",
                    0);

                parameters.Add(
                    "@CustomerId",
                    model.CustomerId);

                parameters.Add(
                    "@SalesInvoiceId",
                    invoice.SalesInvoiceId);

                parameters.Add(
                    "@Amount",
                    invoice.CollectionAmount);

                parameters.Add(
                    "@PaymentDate",
                    model.PaymentDate);

                parameters.Add(
                    "@PaymentMethod",
                    model.PaymentMethod);

                parameters.Add(
                    "@Remarks",
                    model.Remarks ?? string.Empty);

                parameters.Add(
                    "@UserId",
                    userId);

                await connection.ExecuteAsync(
                    "sp_CustomerPayment_Save",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }

            transaction.Commit();

            return true;
        }
        catch(Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine( ex.Message);
            throw;
            
        }
    }
}