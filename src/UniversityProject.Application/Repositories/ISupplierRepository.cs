using Dapper;
using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ISupplierRepository
{
    Task<long> SaveAsync(Supplier supplier);
    Task<Supplier> GetByIdAsync(long supplierId);
    Task<(List<Supplier> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize);
    Task<bool> DeleteAsync(long supplierId, long deletedBy);
}
public class SupplierRepository : ISupplierRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SupplierRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region Save

    public async Task<long> SaveAsync(Supplier supplier)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameter = new DynamicParameters();

        parameter.Add("@Id", supplier.Id);
        parameter.Add("@Name", supplier.Name);
        parameter.Add("@Phone", supplier.Phone);
        parameter.Add("@Email", supplier.Email);
        parameter.Add("@Address", supplier.Address);
        parameter.Add("@CompanyName", supplier.CompanyName);
        parameter.Add("@ContactPerson", supplier.ContactPerson);
        parameter.Add("@TradeLicense", supplier.TradeLicense);
        parameter.Add("@TIN", supplier.TIN);
        parameter.Add("@BIN", supplier.BIN);
        parameter.Add("@OpeningBalance", supplier.OpeningBalance);

        parameter.Add("@UserId",
            supplier.Id == 0
                ? supplier.CreatedBy
                : supplier.ModifiedBy);

        return await connection.ExecuteScalarAsync<long>(
            "sp_Supplier_Save",
            parameter,
            commandType: CommandType.StoredProcedure);
    }

    #endregion

    #region Get By Id

    public async Task<Supplier> GetByIdAsync(long supplierId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Supplier>(
            "sp_Supplier_GetById",
            new
            {
                SupplierId = supplierId
            },
            commandType: CommandType.StoredProcedure);
    }

    #endregion

    #region List

    public async Task<(List<Supplier> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_Supplier_List",
            new
            {
                Search = search,
                PageNo = pageNo,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var suppliers = (await multi.ReadAsync<Supplier>()).ToList();

        var totalCount = await multi.ReadFirstAsync<int>();

        return (suppliers, totalCount);
    }

    #endregion

    #region Delete

    public async Task<bool> DeleteAsync(long supplierId, long deletedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstOrDefaultAsync<DeleteResult>(
            "sp_Supplier_Delete",
            new
            {
                SupplierId = supplierId,
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