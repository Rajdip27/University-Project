using UniversityProject.Core.Entities;
using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities
{
    public class Purchase : AuditableEntity
    {
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Vat { get; set; }
        public decimal TransportCost { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PurchaseItem> PurchaseItem { get; set; } = new List<PurchaseItem>();
        public ICollection<SupplierPayment> SupplierPayment { get; set; } = new List<SupplierPayment>();
    }
}
