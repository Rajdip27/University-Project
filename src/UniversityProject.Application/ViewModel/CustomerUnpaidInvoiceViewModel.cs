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
public class CustomerPaymentListViewModel
{
    public long Id { get; set; }

    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }

    public long SalesInvoiceId { get; set; }
    public string InvoiceNo { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; }

    public string Remarks { get; set; }

    public long? CreatedBy { get; set; }
    public DateTimeOffset? CreatedDate { get; set; }
}

public class SupplierLedgerReportDto
{
    public SupplierLedgerSummaryDto Summary { get; set; } = new();

    public List<SupplierLedgerTransactionDto> Transactions { get; set; } = new();

    public long? SupplierId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class SupplierLedgerSummaryDto
{
    public decimal OpeningBalance { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class SupplierLedgerTransactionDto
{
    public DateTime TransactionDate { get; set; }

    public long SupplierId { get; set; }
    public string SupplierName { get; set; }

    public string SupplierPhone { get; set; }

    public string ReferenceType { get; set; }
    public long ReferenceId { get; set; }

    public string Description { get; set; }

    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    public decimal RunningBalance { get; set; }
}