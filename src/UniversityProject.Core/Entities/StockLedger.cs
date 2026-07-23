using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class StockLedger : AuditableEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ReferenceType { get; set; } // Purchase/Sale/Return
    public long ReferenceId { get; set; }
    public decimal StockIn { get; set; }
    public decimal StockOut { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceQty { get; set; }
}