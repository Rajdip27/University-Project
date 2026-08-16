using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Reflection.Metadata;
using System.Text.Json;
using UniversityProject.Application.ViewModel;
using UniversityProject.Core.Entities;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ISalesInvoiceRepository
{
    Task<SelectList> GetCustomerDropdownAsync();

    Task<SelectList> GetProductDropdownAsync();

    Task<long> SaveAsync(SalesInvoice invoice);

    Task<SalesInvoice> GetByIdAsync(long invoiceId);

    Task<SalesInvoiceDetailsViewModel> GetDetailsAsync(long invoiceId);

    Task<(List<SalesInvoiceListViewModel> Items, int TotalCount)> GetListAsync(
        string? search,
        int pageNo,
        int pageSize);
    Task<SalesInvoiceDetailsViewModel> GetDetailsByIdAsync(long id);

    Task<bool> DeleteAsync(
        long invoiceId,
        long deletedBy);
}
public class SalesInvoiceRepository : ISalesInvoiceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SalesInvoiceRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<SalesInvoiceDetailsViewModel> GetDetailsByIdAsync(long id)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_SalesInvoice_GetById",
            new
            {
                SalesInvoiceId = id
            },
            commandType: CommandType.StoredProcedure
        );

        var invoice = await multi.ReadFirstOrDefaultAsync<SalesInvoiceDetailsViewModel>();

        if (invoice == null)
            return null;

        var items = (await multi.ReadAsync<SalesItemDetailsViewModel>())
            .ToList();

        invoice.Items = items;

        return invoice;
    }

    // =========================================================
    // CUSTOMER DROPDOWN
    // =========================================================

    public async Task<SelectList> GetCustomerDropdownAsync()
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var customers = await connection.QueryAsync<Customer>(
            "sp_Customer_Dropdown",
            commandType: CommandType.StoredProcedure);

        return new SelectList(
            customers,
            "Id",
            "Name"
        );
    }


    // =========================================================
    // PRODUCT DROPDOWN
    // =========================================================

    public async Task<SelectList> GetProductDropdownAsync()
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var products = await connection.QueryAsync<Product>(
            "sp_Product_Dropdown",
            commandType: CommandType.StoredProcedure);

        return new SelectList(
            products,
            "Id",
            "ProductName"
        );
    }


    // =========================================================
    // SAVE / UPDATE
    // =========================================================

    public async Task<long> SaveAsync(
        SalesInvoice invoice)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@SalesInvoiceId",
            invoice.Id);

        parameters.Add(
            "@InvoiceNo",
            invoice.InvoiceNo);

        parameters.Add(
            "@CustomerId",
            invoice.CustomerId);

        parameters.Add(
            "@InvoiceDate",
            invoice.InvoiceDate);

        parameters.Add(
            "@Discount",
            invoice.Discount);

        parameters.Add(
            "@Tax",
            invoice.Tax);

        parameters.Add(
            "@Vat",
            invoice.Vat);

        parameters.Add(
            "@GrandTotal",
            invoice.GrandTotal);

        parameters.Add(
            "@PaidAmount",
            invoice.PaidAmount);

        parameters.Add(
            "@DueAmount",
            invoice.DueAmount);
        parameters.Add("@WarehouseId", invoice.WarehouseId);


        parameters.Add(
            "@UserId",
            invoice.Id == 0
                ? invoice.CreatedBy
                : invoice.ModifiedBy);


        // -----------------------------------------------------
        // ITEMS JSON
        // -----------------------------------------------------

        var items = invoice.SalesItem
            .Select(x => new
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            })
            .ToList();

        var itemsJson =
            JsonSerializer.Serialize(items);

        parameters.Add(
            "@SalesItems",
            itemsJson);


        return await connection.ExecuteScalarAsync<long>(
            "sp_SalesInvoice_Save",
            parameters,
            commandType: CommandType.StoredProcedure);
    }


    // =========================================================
    // GET BY ID - EDIT
    // =========================================================

    public async Task<SalesInvoice> GetByIdAsync(
        long invoiceId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        using var multi =
            await connection.QueryMultipleAsync(
                "sp_SalesInvoice_GetById",
                new
                {
                    SalesInvoiceId = invoiceId
                },
                commandType: CommandType.StoredProcedure);


        var invoice =
            await multi.ReadFirstOrDefaultAsync<SalesInvoice>();

        if (invoice == null)
            return null;


        var items =
            (await multi.ReadAsync<SalesItem>())
            .ToList();

        invoice.SalesItem = items;

        return invoice;
    }


    // =========================================================
    // DETAILS
    // =========================================================

    public async Task<SalesInvoiceDetailsViewModel?>
        GetDetailsAsync(long invoiceId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        using var multi =
            await connection.QueryMultipleAsync(
                "sp_SalesInvoice_GetById",
                new
                {
                    SalesInvoiceId = invoiceId
                },
                commandType: CommandType.StoredProcedure);


        // Master
        var invoice =
            await multi.ReadFirstOrDefaultAsync<
                SalesInvoiceDetailsViewModel>();

        if (invoice == null)
            return null;


        // Items
        var items =
            (await multi.ReadAsync<
                SalesItemDetailsViewModel>())
            .ToList();


        invoice.Items = items;

        return invoice;
    }


    // =========================================================
    // LIST
    // =========================================================
    public async Task<(List<SalesInvoiceListViewModel> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_SalesInvoice_List",
            new
            {
                Search = search,
                PageNo = pageNo,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var invoices =
            (await multi.ReadAsync<SalesInvoiceListViewModel>())
            .ToList();

        var totalCount =
            await multi.ReadFirstAsync<int>();

        return (
            invoices,
            totalCount
        );
    }


    // =========================================================
    // DELETE
    // =========================================================

    public async Task<bool> DeleteAsync(
        long invoiceId,
        long deletedBy)
    {
        using var connection =
            _connectionFactory.CreateConnection();


        var result =
            await connection.QueryFirstOrDefaultAsync<DeleteResult>(
                "sp_SalesInvoice_Delete",
                new
                {
                    SalesInvoiceId = invoiceId,
                    DeletedBy = deletedBy
                },
                commandType: CommandType.StoredProcedure);


        return result != null &&
               result.Success;
    }


    // =========================================================
    // DELETE RESULT
    // =========================================================

    private class DeleteResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}