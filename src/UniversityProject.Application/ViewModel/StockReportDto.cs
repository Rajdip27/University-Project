using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityProject.Application.ViewModel;

public class StockReportDto
{
    public decimal OpeningQty { get; set; }

    public decimal TotalIn { get; set; }

    public decimal TotalOut { get; set; }

    public decimal ClosingQty { get; set; }

    public List<StockTransactionDto> Transactions { get; set; }
        = new List<StockTransactionDto>();
}
public class StockTransactionDto
{
    public DateTime TransactionDate { get; set; }

    public string ProductName { get; set; }

    public string ProductCode { get; set; }

    public string WarehouseName { get; set; }

    public string ReferenceType { get; set; }

    public long ReferenceId { get; set; }

    public decimal StockIn { get; set; }

    public decimal StockOut { get; set; }

    public decimal UnitCost { get; set; }

    public decimal RunningBalance { get; set; }
}