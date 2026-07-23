using System.ComponentModel.DataAnnotations;
using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class Product: AuditableEntity
{
    [Required, StringLength(200)]
    public string ProductName { get; set; }
    [StringLength(100)]
    public string Sku { get; set; }
    [StringLength(100)]
    public string Barcode { get; set; }
    public string Description { get; set; }
    [Required]
    public decimal PurchasePrice { get; set; }
    [Required]
    public decimal SellingPrice { get; set; }
    public int WarrantyMonths { get; set; } = 0;
    public bool Status { get; set; } = true;
    public ICollection<StockLedger> StockLedger { get; set; } = new List<StockLedger>();
    public ICollection<PurchaseItem> PurchaseItem { get; set; } = new List<PurchaseItem>();
    public ICollection<SalesItem> SalesItem { get; set; } = new List<SalesItem>();
}
