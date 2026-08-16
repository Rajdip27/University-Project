using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniversityProject.Application.ViewModel;
using UniversityProject.Core.Entities;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Services;

public interface IPurchaseRepository
{
    Task<SelectList> GetDropdownAsync();
    Task<long> SaveAsync(Purchase purchase);
    Task<Purchase> GetByIdAsync(long purchaseId);
    Task<(List<Purchase> Items, int TotalCount)> GetListAsync(
        string search,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNo,
        int pageSize);
    Task<bool> DeleteAsync(long purchaseId, long deletedBy);

    Task<StockReportDto> GetStockReportAsync(
    long? productId,
    long? warehouseId,
    DateTime? startDate,
    DateTime? endDate);
}

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PurchaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<StockReportDto> GetStockReportAsync(
    long? productId,
    long? warehouseId,
    DateTime? startDate,
    DateTime? endDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@ProductId", productId);
        parameters.Add("@WarehouseId", warehouseId);
        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        using var multi = await connection.QueryMultipleAsync(
            "sp_StockLedger_Report",
            parameters,
            commandType: CommandType.StoredProcedure);

        // Result Set 1
        var summary = await multi.ReadFirstOrDefaultAsync<StockReportDto>();

        // Result Set 2
        var transactions = (await multi.ReadAsync<StockTransactionDto>())
            .ToList();

        if (summary == null)
        {
            summary = new StockReportDto();
        }

        summary.Transactions = transactions;

        return summary;
    }
    public async Task<SelectList> GetDropdownAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var warehouses = await connection.QueryAsync<Warehouse>(
            "sp_Warehouse_Dropdown",
            commandType: CommandType.StoredProcedure);

        return new SelectList(
            warehouses,
            "Id",
            "Name"
        );
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

        var parameters = new DynamicParameters();

        parameters.Add("@Search", search);
        parameters.Add("@FromDate", fromDate);
        parameters.Add("@ToDate", toDate);
        parameters.Add("@PageNo", pageNo);
        parameters.Add("@PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_Purchase_List",
            parameters,
            commandType: CommandType.StoredProcedure);

        // First result set
        var items = (await multi.ReadAsync<PurchaseListDto>())
            .ToList();

        // Second result set
        var totalCount = await multi.ReadFirstOrDefaultAsync<int>();

        // Map Supplier and Warehouse navigation properties
        var purchases = items.Select(x => new Purchase
        {
            Id = x.Id,
            InvoiceNo = x.InvoiceNo,
            SupplierId = x.SupplierId,
            WarehouseId = x.WarehouseId,
            PurchaseDate = x.PurchaseDate,
            Discount = x.Discount,
            Tax = x.Tax,
            Vat = x.Vat,
            TransportCost = x.TransportCost,
            GrandTotal = x.GrandTotal,
            CreatedDate = x.CreatedDate,

            Supplier = new Supplier
            {
                Id = x.SupplierId,
                Name = x.SupplierName
            },

            Warehouse = new Warehouse
            {
                Id = x.WarehouseId,
                Name = x.WarehouseName
            }

        }).ToList();

        return (purchases, totalCount);
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
