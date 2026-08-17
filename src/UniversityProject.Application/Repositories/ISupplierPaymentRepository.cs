using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityProject.Application.ViewModel;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ISupplierPaymentRepository
{
    Task<(List<SupplierPaymentListViewModel> Items, int TotalCount)> GetListAsync(string search,int pageNo, int pageSize);

    Task<List<SupplierUnpaidPurchaseViewModel>>
        GetSupplierUnpaidPurchases(
            long supplierId);

    Task<bool> SaveAsync(
        SupplierPaymentViewModel model,
        long userId);
}
public class SupplierPaymentRepository
    : ISupplierPaymentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SupplierPaymentRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    // ==========================================
    // PAYMENT LIST
    // ==========================================

    public async Task<(
        List<SupplierPaymentListViewModel> Items,
        int TotalCount)>
        GetListAsync(
            string search,
            int pageNo,
            int pageSize)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        if (connection.State != ConnectionState.Open)
            connection.Open();


        using var multi =
            await connection.QueryMultipleAsync(
                "sp_SupplierPayment_List",
                new
                {
                    Search = search,
                    PageNo = pageNo,
                    PageSize = pageSize
                },
                commandType:
                    CommandType.StoredProcedure);


        var items =
            (await multi.ReadAsync<
                SupplierPaymentListViewModel>())
            .ToList();


        var totalCount =
            await multi.ReadFirstAsync<int>();


        return (
            items,
            totalCount
        );
    }


    // ==========================================
    // UNPAID PURCHASES
    // ==========================================

    public async Task<List<SupplierUnpaidPurchaseViewModel>>
        GetSupplierUnpaidPurchases(
            long supplierId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        if (connection.State != ConnectionState.Open)
            connection.Open();


        var result =
            await connection.QueryAsync<
                SupplierUnpaidPurchaseViewModel>(
                "sp_Supplier_UnpaidPurchases",
                new
                {
                    SupplierId = supplierId
                },
                commandType:
                    CommandType.StoredProcedure);


        return result.ToList();
    }


    // ==========================================
    // SAVE PAYMENT
    // ==========================================

    public async Task<bool> SaveAsync(
        SupplierPaymentViewModel model,
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
            var payments =
                model.Purchases
                    .Where(x => x.CollectionAmount > 0)
                    .Select(x => new
                    {
                        PurchaseId = x.PurchaseId,
                        Amount = x.CollectionAmount
                    })
                    .ToList();


            var json =
                System.Text.Json.JsonSerializer
                    .Serialize(payments);


            var parameters =
                new DynamicParameters();


            parameters.Add(
                "@SupplierId",
                model.SupplierId);


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


            parameters.Add(
                "@Payments",
                json);


            await connection.ExecuteAsync(
                "sp_SupplierPayment_Save",
                parameters,
                transaction,
                commandType:
                    CommandType.StoredProcedure);


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