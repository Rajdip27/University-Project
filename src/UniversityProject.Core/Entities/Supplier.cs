using UniversityProject.Core.Entities;
using UniversityProject.Core.Entities.BaseEntities;

namespace InventorySystem.Models;

public class Supplier : AuditableEntity
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string CompanyName { get; set; }
    public string ContactPerson { get; set; }
    public string TradeLicense { get; set; }
    public string TIN { get; set; }
    public string BIN { get; set; }
    public decimal OpeningBalance { get; set; } = 0;
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<SupplierLedger> SupplierLedgers { get; set; }= new List<SupplierLedger>();
    public ICollection<SupplierPayment> SupplierPayment { get; set; }= new List<SupplierPayment>();
}
