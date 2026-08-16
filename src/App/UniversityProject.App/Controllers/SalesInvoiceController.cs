using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.Services;
using UniversityProject.Core.Entities;

namespace UniversityProject.App.Controllers;

public class SalesInvoiceController(
    ISalesInvoiceRepository _salesInvoiceService,
    IAppLogger<SalesInvoiceController> _logger,IPurchaseRepository purchaseRepository) : Controller
{
    #region List

    [HttpGet]
    public async Task<IActionResult> Index(string search,int page = 1)
    {
        _logger.LogInfo(
            $"Loading Sales Invoice List. Search={search} Page={page}");

        try
        {
            var result = await _salesInvoiceService.GetListAsync(search, page,10);

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalCount = result.TotalCount;
            return View(result.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load Sales Invoice list.", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Failed to load sales invoices.";

            return View(new List<SalesInvoice>());
        }
    }

    #endregion


    #region Create

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        _logger.LogInfo("Sales Invoice Create Page");

        await LoadDropdowns();
        ViewBag.Warehouses = await purchaseRepository.GetDropdownAsync();
        var model = new SalesInvoice
        {
            InvoiceDate = DateTime.Now
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalesInvoice model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Invalid Sales Invoice create request.");

            await LoadDropdowns();

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] =
                "Please enter valid sales invoice information.";

            return View(model);
        }

        try
        {
            model.CreatedBy = 1;

            var id = await _salesInvoiceService.SaveAsync(model);

            _logger.LogInfo(
                $"Sales Invoice created successfully. InvoiceId={id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] =
                "Sales invoice created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Sales Invoice create failed. InvoiceNo={model.InvoiceNo}",
                ex);

            await LoadDropdowns();

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(model);
        }
    }

    #endregion


    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        _logger.LogInfo(
            $"Loading Sales Invoice Edit. InvoiceId={id}");

        try
        {
            var invoice =
                await _salesInvoiceService.GetByIdAsync(id);

            if (invoice == null)
            {
                _logger.LogWarning(
                    $"Sales Invoice not found. InvoiceId={id}");

                TempData["AlertType"] = "warning";
                TempData["AlertMessage"] =
                    "Sales invoice not found.";

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            ViewBag.Warehouses = await purchaseRepository.GetDropdownAsync();

            return View(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Failed to load Sales Invoice. InvoiceId={id}",
                ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SalesInvoice model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                $"Invalid Sales Invoice update request. InvoiceId={model.Id}");

            await LoadDropdowns();

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] =
                "Please enter valid sales invoice information.";

            return View(model);
        }

        try
        {
            model.ModifiedBy = 1;

            var id = await _salesInvoiceService.SaveAsync(model);

            _logger.LogInfo(
                $"Sales Invoice updated successfully. InvoiceId={id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] =
                "Sales invoice updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Sales Invoice update failed. InvoiceId={model.Id}",
                ex);

            await LoadDropdowns();

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(model);
        }
    }

    #endregion


    #region Details

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo(
            $"Loading Sales Invoice Details. InvoiceId={id}");

        try
        {
            var invoice =
                await _salesInvoiceService.GetDetailsByIdAsync(id);

            if (invoice == null)
            {
                _logger.LogWarning(
                    $"Sales Invoice not found. InvoiceId={id}");

                TempData["AlertType"] = "warning";
                TempData["AlertMessage"] =
                    "Sales invoice not found.";

                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Failed to load Sales Invoice Details. InvoiceId={id}",
                ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }

    #endregion


    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        _logger.LogInfo(
            $"Deleting Sales Invoice. InvoiceId={id}");

        try
        {
            var result =
                await _salesInvoiceService.DeleteAsync(id, 1);

            if (result)
            {
                _logger.LogInfo(
                    $"Sales Invoice deleted successfully. InvoiceId={id}");

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] =
                    "Sales invoice deleted successfully.";
            }
            else
            {
                _logger.LogWarning(
                    $"Sales Invoice delete failed. InvoiceId={id}");

                TempData["AlertType"] = "error";
                TempData["AlertMessage"] =
                    "Delete failed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Sales Invoice delete failed. InvoiceId={id}",
                ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Print(long id)
    {
        _logger.LogInfo($"Generating PDF view for Invoice. InvoiceId={id}");

        try
        {
            // Assuming GetByIdAsync returns the SalesInvoiceDetailsViewModel
            // If your repository returns the Entity instead of the ViewModel, 
            // you will need to map it to SalesInvoiceDetailsViewModel here.
            var invoice = await _salesInvoiceService.GetDetailsByIdAsync(id);

            if (invoice == null)
            {
                _logger.LogWarning($"Sales Invoice not found. InvoiceId={id}");
                TempData["AlertType"] = "warning";
                TempData["AlertMessage"] = "Sales invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load PDF view. InvoiceId={id}", ex);
            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion


    #region Dropdowns

    private async Task LoadDropdowns()
    {
        ViewBag.Customers =
            await _salesInvoiceService.GetCustomerDropdownAsync();

        ViewBag.Products =
            await _salesInvoiceService.GetProductDropdownAsync();
    }

    #endregion
}