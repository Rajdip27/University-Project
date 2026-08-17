using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.Services;
using UniversityProject.Core.Entities;

namespace UniversityProject.App.Controllers;

public class PurchaseController(
    IPurchaseRepository _purchaseService,
    ISupplierRepository _supplierService,
    IProductRepository _productService,
    IAppLogger<PurchaseController> _logger) : Controller
{
    #region List

    public async Task<IActionResult> Index(
        string search,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1)
    {
        try
        {
            _logger.LogInfo($"Loading Purchase List. Search={search}");

            var result = await _purchaseService.GetListAsync(
                search,
                fromDate,
                toDate,
                page,
                10);

            ViewBag.Search = search;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Page = page;
            ViewBag.TotalCount = result.TotalCount;

            return View(result.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError( "Error loading purchase list.", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Unable to load purchase list.";

            return View(new List<Purchase>());
        }
    }

    #endregion

    #region Create

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        _logger.LogInfo("Purchase Create Page");
        ViewBag.Suppliers = await _supplierService.GetDropdownAsync();

        ViewBag.Products = await _productService.GetDropdownAsync();
        ViewBag.Warehouses = await _purchaseService.GetDropdownAsync();

        return View(new Purchase());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Purchase purchase)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Please enter valid information.";

            return View(purchase);
        }

        try
        {
            purchase.CreatedBy = 1;

            var id = await _purchaseService.SaveAsync(purchase);

            _logger.LogInfo($"Purchase Created. Id={id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Purchase saved successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( "Purchase Save Failed.", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(purchase);
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var purchase = await _purchaseService.GetByIdAsync(id);
        ViewBag.Suppliers = await _supplierService.GetDropdownAsync();

        ViewBag.Products = await _productService.GetDropdownAsync();
        ViewBag.Warehouses = await _purchaseService.GetDropdownAsync();

        if (purchase == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Purchase not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(purchase);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Purchase purchase)
    {
        if (!ModelState.IsValid)
            return View(purchase);

        try
        {
            purchase.ModifiedBy = 1;

            await _purchaseService.SaveAsync(purchase);

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Purchase updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( "Purchase Update Failed.", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(purchase);
        }
    }

    #endregion

    #region Details

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        ViewBag.Suppliers = await _supplierService.GetDropdownAsync();

        ViewBag.Products = await _productService.GetDropdownAsync();
        ViewBag.Warehouses = await _purchaseService.GetDropdownAsync();
        var purchase = await _purchaseService.GetDetailsAsync(id);

        if (purchase == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Purchase not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(purchase);
    }

    #endregion

    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _purchaseService.DeleteAsync(id, 1);

            if (result)
            {
                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Purchase deleted successfully.";
            }
            else
            {
                TempData["AlertType"] = "error";
                TempData["AlertMessage"] = "Delete failed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Purchase Delete Failed. Id={id}", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    

    #endregion
}