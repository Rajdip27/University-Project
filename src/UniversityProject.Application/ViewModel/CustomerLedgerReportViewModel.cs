using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityProject.Application.ViewModel;

public class CustomerLedgerReportViewModel
{
    public DateTime TransactionDate { get; set; }

    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerAddress { get; set; }

    public string ReferenceType { get; set; }
    public long? ReferenceId { get; set; }
    public string Description { get; set; }

    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal RunningBalance { get; set; }
}
public class CustomerLedgerReportSummaryViewModel
{
    public decimal OpeningBalance { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class CustomerLedgerReportDto
{
    public CustomerLedgerReportSummaryViewModel Summary { get; set; }
        = new CustomerLedgerReportSummaryViewModel();

    public List<CustomerLedgerReportViewModel> Transactions { get; set; }
        = new List<CustomerLedgerReportViewModel>();
}