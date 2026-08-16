namespace UniversityProject.Application.ViewModel;

public class CustomerPaymentViewModel
{
    public long CustomerId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    public string PaymentMethod { get; set; } = "Cash";

    public string Remarks { get; set; } = string.Empty;

    public decimal TotalPayment { get; set; }

    public List<CustomerUnpaidInvoiceViewModel> Invoices { get; set; }
        = new List<CustomerUnpaidInvoiceViewModel>();
}

public class CustomerUnpaidInvoiceViewModel
{
    public long SalesInvoiceId { get; set; }

    public string InvoiceNo { get; set; }

    public string CustomerName { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public decimal RemainingDueAmount { get; set; }

    // User entered collection amount
    public decimal CollectionAmount { get; set; }
}