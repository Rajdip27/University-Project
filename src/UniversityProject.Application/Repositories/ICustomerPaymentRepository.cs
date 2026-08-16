using Dapper;
using System.Data;
using UniversityProject.Application.ViewModel;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ICustomerPaymentRepository
{
    Task<List<CustomerUnpaidInvoiceViewModel>> GetCustomerUnpaidInvoices(long customerId);

    Task<bool> SaveAsync(
        CustomerPaymentViewModel model,
        long userId);
}

public class CustomerPaymentRepository : ICustomerPaymentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerPaymentRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    //====================================================
    // GET CUSTOMER UNPAID INVOICES
    //====================================================

    public async Task<List<CustomerUnpaidInvoiceViewModel>>
        GetCustomerUnpaidInvoices(long customerId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        if (connection.State != ConnectionState.Open)
            connection.Open();

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
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}