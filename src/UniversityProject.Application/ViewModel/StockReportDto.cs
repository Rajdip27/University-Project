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

public class PurchaseListViewModel
{
    public long Id { get; set; }

    public string InvoiceNo { get; set; }

    public long SupplierId { get; set; }

    public string SupplierName { get; set; }

    public long WarehouseId { get; set; }

    public string WarehouseName { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Vat { get; set; }

    public decimal TransportCost { get; set; }

    public decimal GrandTotal { get; set; }

    //public DateTimeOffset? CreatedDate { get; set; }
}

public class PurchaseDetailsViewModel
{
    public long Id { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;

    public long SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public long WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Vat { get; set; }

    public decimal TransportCost { get; set; }

    public decimal GrandTotal { get; set; }

    //public DateTime? CreatedDate { get; set; }

    public List<PurchaseDetailsItemViewModel> Items { get; set; }
        = new();
}


public class PurchaseDetailsItemViewModel
{
    public long Id { get; set; }

    public long PurchaseId { get; set; }

    public long ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total => Quantity * UnitPrice;
}