using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.ViewModel;

namespace UniversityProject.App.Controllers;

public class SupplierPaymentsController(
    ISupplierPaymentRepository _paymentRepository,
    ISupplierRepository _supplierRepository,
    IAppLogger<SupplierPaymentsController> _logger)
    : Controller
{
    // ==========================================
    // INDEX
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Index(
        string search,
        int page = 1)
    {
        try
        {
            var result =
                await _paymentRepository.GetListAsync(
                    search,
                    page,
                    10);


            ViewBag.Search = search;

            ViewBag.Page = page;

            ViewBag.TotalCount =
                result.TotalCount;


            return View(result.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to load supplier payments.",
                ex);


            TempData["AlertType"] = "error";

            TempData["AlertMessage"] =
                "Failed to load supplier payment history.";


            return View(
                new List<SupplierPaymentListViewModel>());
        }
    }


    // ==========================================
    // CREATE GET
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.Suppliers =
                await _supplierRepository
                    .GetDropdownAsync();


            var model =
                new SupplierPaymentViewModel
                {
                    PaymentDate = DateTime.Now,

                    PaymentMethod = "Cash",

                    Purchases =
                        new List<
                            SupplierUnpaidPurchaseViewModel>()
                };


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Supplier payment create page failed.",
                ex);


            TempData["AlertType"] = "error";

            TempData["AlertMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Index));
        }
    }


    // ==========================================
    // LOAD UNPAID PURCHASES
    // ==========================================

    [HttpGet]
    public async Task<IActionResult>
        GetSupplierPurchases(
            long supplierId)
    {
        if (supplierId <= 0)
        {
            return Json(
                new List<
                    SupplierUnpaidPurchaseViewModel>());
        }


        try
        {
            var purchases =
                await _paymentRepository
                    .GetSupplierUnpaidPurchases(
                        supplierId);


            return Json(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Failed loading unpaid purchases. " +
                $"SupplierId={supplierId}",
                ex);


            return StatusCode(
                500,
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==========================================
    // CREATE POST
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SupplierPaymentViewModel model)
    {
        try
        {
            if (model.SupplierId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.SupplierId),
                    "Please select supplier.");
            }


            var selectedPayments =
                model.Purchases?
                    .Where(x =>
                        x.CollectionAmount > 0)
                    .ToList();


            if (selectedPayments == null ||
                selectedPayments.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter at least one payment amount.");
            }


            if (selectedPayments != null)
            {
                foreach (var purchase
                    in selectedPayments)
                {
                    if (purchase.CollectionAmount >
                        purchase.DueAmount)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Payment amount for purchase " +
                            $"{purchase.InvoiceNo} cannot exceed " +
                            $"remaining due amount.");
                    }


                    if (purchase.CollectionAmount <= 0)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Invalid payment amount for purchase " +
                            $"{purchase.InvoiceNo}.");
                    }
                }
            }


            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers =
                    await _supplierRepository
                        .GetDropdownAsync();

                return View(model);
            }


            model.Purchases =
                selectedPayments;


            model.TotalPayment =
                selectedPayments.Sum(
                    x => x.CollectionAmount);


            long userId = 1;


            var result =
                await _paymentRepository.SaveAsync(
                    model,
                    userId);


            if (result)
            {
                _logger.LogInfo(
                    $"Supplier payment saved. " +
                    $"SupplierId={model.SupplierId}, " +
                    $"Amount={model.TotalPayment}");


                TempData["AlertType"] =
                    "success";


                TempData["AlertMessage"] =
                    "Supplier payment saved successfully.";


                return RedirectToAction(
                    nameof(Create));
            }


            TempData["AlertType"] =
                "error";


            TempData["AlertMessage"] =
                "Payment save failed.";


            ViewBag.Suppliers =
                await _supplierRepository
                    .GetDropdownAsync();


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Supplier payment save failed. " +
                $"SupplierId={model.SupplierId}",
                ex);


            TempData["AlertType"] =
                "error";


            TempData["AlertMessage"] =
                ex.Message;


            ViewBag.Suppliers =
                await _supplierRepository
                    .GetDropdownAsync();


            return View(model);
        }
    }
}