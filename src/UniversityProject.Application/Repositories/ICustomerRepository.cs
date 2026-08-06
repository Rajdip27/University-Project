using Dapper;
using System.Data;
using UniversityProject.Core.Entities;
using UniversityProject.Infrastructure.Dapper;

namespace UniversityProject.Application.Repositories;

public interface ICustomerRepository
{
    Task<long> SaveAsync(Customer customer);
    Task<Customer> GetByIdAsync(long customerId);
    Task<(List<Customer> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize);

    Task<bool> DeleteAsync(long customerId, long deletedBy);
}
public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region Save

    public async Task<long> SaveAsync(Customer customer)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameter = new DynamicParameters();

        parameter.Add("@CustomerId", customer.Id);
        parameter.Add("@Name", customer.Name);
        parameter.Add("@Phone", customer.Phone);
        parameter.Add("@Email", customer.Email);
        parameter.Add("@Address", customer.Address);
        parameter.Add("@OpeningBalance", customer.OpeningBalance);

        parameter.Add("@UserId",
            customer.Id == 0
                ? customer.CreatedBy
                : customer.ModifiedBy);

        return await connection.ExecuteScalarAsync<long>(
            "sp_Customer_Save",
            parameter,
            commandType: CommandType.StoredProcedure);
    }

    #endregion

    #region Get By Id

    public async Task<Customer> GetByIdAsync(long customerId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Customer>(
            "sp_Customer_GetById",
            new
            {
                CustomerId = customerId
            },
            commandType: CommandType.StoredProcedure);
    }

    #endregion

    #region List

    public async Task<(List<Customer> Items, int TotalCount)> GetListAsync(
        string search,
        int pageNo,
        int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_Customer_List",
            new
            {
                Search = search,
                PageNo = pageNo,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var customers = (await multi.ReadAsync<Customer>()).ToList();

        var totalCount = await multi.ReadFirstAsync<int>();

        return (customers, totalCount);
    }

    #endregion

    #region Delete

    public async Task<bool> DeleteAsync(long customerId, long deletedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstOrDefaultAsync<DeleteResult>(
            "sp_Customer_Delete",
            new
            {
                CustomerId = customerId,
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