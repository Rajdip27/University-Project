using UniversityProject.Core.Entities;
using UniversityProject.Core.Entities.BaseEntities;

namespace InventorySystem.Models;

public class SupplierPayment:AuditableEntity
{
    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public long PurchaseId { get; set; }
    public Purchase Purchase { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
}
