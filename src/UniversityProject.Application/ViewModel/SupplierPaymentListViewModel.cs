using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityProject.Application.ViewModel;

public class SupplierPaymentViewModel
{
    public long SupplierId { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = "Cash";

    public string Remarks { get; set; } = string.Empty;

    public decimal TotalPayment { get; set; }

    public List<SupplierUnpaidPurchaseViewModel> Purchases { get; set; }
        = new();
}
public class SupplierUnpaidPurchaseViewModel
{
    public long PurchaseId { get; set; }

    public string InvoiceNo { get; set; }

    public long SupplierId { get; set; }

    public string SupplierName { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public decimal CollectionAmount { get; set; }
}
public class SupplierPaymentListViewModel
{
    public long Id { get; set; }

    public long SupplierId { get; set; }

    public string SupplierName { get; set; }

    public long PurchaseId { get; set; }

    public string InvoiceNo { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; }

    public string Remarks { get; set; }
}