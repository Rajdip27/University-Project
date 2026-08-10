using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityProject.Application.ViewModel;

public class PurchaseListDto
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

    public DateTimeOffset CreatedDate { get; set; }
}
