using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniversityProject.Core.Entities;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Services;

public interface IPurchaseRepository
{
    Task<long> SaveAsync(Purchase purchase);
    Task<Purchase> GetByIdAsync(long purchaseId);
    Task<(List<Purchase> Items, int TotalCount)> GetListAsync(
        string search,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNo,
        int pageSize);
    Task<bool> DeleteAsync(long purchaseId, long deletedBy);
}

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PurchaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region Save

    public async Task<long> SaveAsync(Purchase purchase)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameter = new DynamicParameters();

        parameter.Add("@PurchaseId", purchase.Id);
        parameter.Add("@InvoiceNo", purchase.InvoiceNo);
        parameter.Add("@SupplierId", purchase.SupplierId);
        parameter.Add("@WarehouseId", purchase.WarehouseId);
        parameter.Add("@PurchaseDate", purchase.PurchaseDate);

        parameter.Add("@Discount", purchase.Discount);
        parameter.Add("@Tax", purchase.Tax);
        parameter.Add("@Vat", purchase.Vat);
        parameter.Add("@TransportCost", purchase.TransportCost);
        parameter.Add("@GrandTotal", purchase.GrandTotal);

        parameter.Add("@UserId",
            purchase.Id == 0
                ? purchase.CreatedBy
                : purchase.ModifiedBy);

        var items = purchase.PurchaseItem.Select(x => new
        {
            x.ProductId,
            x.Quantity,
            x.UnitPrice
        });

        parameter.Add(
            "@PurchaseItems",
            JsonSerializer.Serialize(items),
            DbType.String);

        return await connection.ExecuteScalarAsync<long>(
            "sp_Purchase_Save",
            parameter,
            commandType: CommandType.StoredProcedure);
    }

    #endregion

    #region Get By Id

    public async Task<Purchase> GetByIdAsync(long purchaseId)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_Purchase_GetById",
            new
            {
                PurchaseId = purchaseId
            },
            commandType: CommandType.StoredProcedure);

        var purchase = await multi.ReadFirstOrDefaultAsync<Purchase>();

        if (purchase == null)
            return null;

        purchase.PurchaseItem = (await multi.ReadAsync<PurchaseItem>()).ToList();

        return purchase;
    }

    #endregion

    #region List

    public async Task<(List<Purchase> Items, int TotalCount)> GetListAsync(
        string search,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNo,
        int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_Purchase_List",
            new
            {
                Search = search,
                FromDate = fromDate,
                ToDate = toDate,
                PageNo = pageNo,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var list = (await multi.ReadAsync<Purchase>()).ToList();

        var totalCount = await multi.ReadFirstAsync<int>();

        return (list, totalCount);
    }

    #endregion

    #region Delete

    public async Task<bool> DeleteAsync(long purchaseId, long deletedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstOrDefaultAsync<DeleteResult>(
            "sp_Purchase_Delete",
            new
            {
                PurchaseId = purchaseId,
                DeletedBy = deletedBy
            },
            commandType: CommandType.StoredProcedure);

        return result != null && result.Success;
    }

    #endregion

    private class DeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
