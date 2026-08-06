using UniversityProject.Core.Entities;
using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class SupplierLedger:AuditableEntity
{
    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ReferenceType { get; set; }
    public long? ReferenceId { get; set; }
    public string Description { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal ClosingBalance { get; set; }
}
