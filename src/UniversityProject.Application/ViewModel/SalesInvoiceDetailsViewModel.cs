using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityProject.Core.Entities.BaseEntities;

namespace UniversityProject.Application.ViewModel;

public class SalesInvoiceDetailsViewModel: BaseEntity
{
    public string InvoiceNo { get; set; }

    public long CustomerId { get; set; }
    public long WarehouseId { get; set; }

    public string CustomerName { get; set; }

    public string CustomerPhone { get; set; }

    public string CustomerEmail { get; set; }

    public string CustomerAddress { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Vat { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public List<SalesItemDetailsViewModel> Items { get; set; }
        = new List<SalesItemDetailsViewModel>();
}
public class SalesItemDetailsViewModel:BaseEntity
{

    public long SalesInvoiceId { get; set; }

    public long ProductId { get; set; }

    public string ProductName { get; set; }

    public string Sku { get; set; }

    public string Barcode { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }
}

public class SalesInvoiceListViewModel
{
    public long Id { get; set; }
    public string InvoiceNo { get; set; }

    public long CustomerId { get; set; }
    public string CustomerName { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Vat { get; set; }

    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}