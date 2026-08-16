using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityProject.Core.Entities;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface IProductRepository
{
    Task<SelectList> GetDropdownAsync();
    Task<long> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(long id, long userId);
    Task<Product> GetByIdAsync(long id);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize);
}
public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<SelectList> GetDropdownAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var products = await connection.QueryAsync<Product>(
            "sp_Product_Dropdown",
            commandType: CommandType.StoredProcedure);

        return new SelectList(
            products,
            "Id",
            "ProductName"
        );
    }
    public async Task<long> AddAsync(Product product)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@ProductName", product.ProductName);
        parameters.Add("@Sku", product.Sku);
        parameters.Add("@Barcode", product.Barcode);
        parameters.Add("@Description", product.Description);
        parameters.Add("@PurchasePrice", product.PurchasePrice);
        parameters.Add("@SellingPrice", product.SellingPrice);
        parameters.Add("@WarrantyMonths", product.WarrantyMonths);
        parameters.Add("@Status", product.Status);
        parameters.Add("@CreatedBy", product.CreatedBy);

        var id = await connection.ExecuteScalarAsync<long>(
            "sp_Product_Insert",
            parameters,
            commandType: CommandType.StoredProcedure);

        return id;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@Id", product.Id);
        parameters.Add("@ProductName", product.ProductName);
        parameters.Add("@Sku", product.Sku);
        parameters.Add("@Barcode", product.Barcode);
        parameters.Add("@Description", product.Description);
        parameters.Add("@PurchasePrice", product.PurchasePrice);
        parameters.Add("@SellingPrice", product.SellingPrice);
        parameters.Add("@WarrantyMonths", product.WarrantyMonths);
        parameters.Add("@Status", product.Status);
        parameters.Add("@ModifiedBy", product.ModifiedBy);

        var row = await connection.ExecuteAsync(
            "sp_Product_Update",
            parameters,
            commandType: CommandType.StoredProcedure);

        return row > 0;
    }

    public async Task<bool> DeleteAsync(long id, long userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var row = await connection.ExecuteAsync(
            "sp_Product_Delete",
            new
            {
                Id = id,
                ModifiedBy = userId
            },
            commandType: CommandType.StoredProcedure);

        return row > 0;
    }

    public async Task<Product> GetByIdAsync(long id)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Product>(
            "sp_Product_GetById",
            new
            {
                Id = id
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_Product_List",
            new
            {
                Search = search,
                PageNo = pageNo,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<Product>()).ToList();

        var total = await multi.ReadFirstAsync<int>();

        return (items, total);
    }
}