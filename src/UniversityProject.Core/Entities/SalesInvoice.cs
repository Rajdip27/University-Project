using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Core.Entities;

public class SalesInvoice:AuditableEntity
{
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }
    public long WarehouseId { get; set; }

    public Customer Customer { get; set; }
    public Warehouse Warehouse { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Vat { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public ICollection<CustomerPayment> CustomerPayment { get; set; } = new List<CustomerPayment>();
    public List<SalesItem> SalesItem { get; set; } = new List<SalesItem>();
}
