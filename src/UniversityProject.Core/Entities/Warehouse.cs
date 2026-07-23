using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class Warehouse: AuditableEntity
{
    public string Name { get; set; }
    public string Address { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<StockLedger> StockLedger { get; set; } = new List<StockLedger>();
}
