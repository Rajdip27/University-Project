
using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class CustomerLedger: AuditableEntity
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public DateTime TransactionDate { get; set; }
    public string ReferenceType { get; set; } // Opening, Invoice, Payment
    public long? ReferenceId { get; set; }
    public string Description { get; set; }
    // Opening Balance before this transaction
    public decimal OpeningBalance { get; set; }
    // Customer owes you
    public decimal Debit { get; set; }
    // Customer paid you
    public decimal Credit { get; set; }

    // Balance after transaction
    public decimal ClosingBalance { get; set; }
}
