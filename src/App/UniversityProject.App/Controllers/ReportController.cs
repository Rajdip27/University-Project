using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.Services;
using UniversityProject.Application.ViewModel;

namespace UniversityProject.App.Controllers;

public class ReportController(
IPurchaseRepository _purchaseService,
IProductRepository _productService,
ICustomerPaymentRepository _CustomerPaymentRepository,
ICustomerRepository _customerRepository,
IAppLogger<PurchaseController> _logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> StockReport(
long? productId,
long? warehouseId,
DateTime? startDate,
DateTime? endDate)
    {
        _logger.LogInfo(
            $"Loading Stock Ledger. " +
            $"ProductId={productId}, " +
            $"WarehouseId={warehouseId}, " +
            $"StartDate={startDate}, " +
            $"EndDate={endDate}");

        var result = await _purchaseService.GetStockReportAsync(
            productId,
            warehouseId,
            startDate,
            endDate);

        ViewBag.Products = await _productService.GetDropdownAsync();
        ViewBag.Warehouses = await _purchaseService.GetDropdownAsync();

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> CustomerLedger(long? customerId, DateTime? startDate, DateTime? endDate)
    {
        var result = await _CustomerPaymentRepository.GetLedgerReportAsync(
            customerId,
            startDate,
            endDate);
        ViewBag.Customers = _customerRepository.GetDropdownAsync();

        var model = new CustomerLedgerReportDto
        {
            Summary = result.Summary,
            Transactions = result.Items
        };

        return View(model);
    }
}
