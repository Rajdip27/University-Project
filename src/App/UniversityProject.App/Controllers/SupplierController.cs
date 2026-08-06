using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Core.Entities;

namespace UniversityProject.App.Controllers;

public class SupplierController(
    ISupplierRepository _supplierService,
    IAppLogger<SupplierController> _logger) : Controller
{
    #region List

    public async Task<IActionResult> Index(string search, int page = 1)
    {
        _logger.LogInfo($"Loading Supplier List. Search={search}, Page={page}");

        var result = await _supplierService.GetListAsync(search, page, 10);

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalCount = result.TotalCount;

        return View(result.Items);
    }

    #endregion

    #region Create

    [HttpGet]
    public IActionResult Create()
    {
        _logger.LogInfo("Supplier Create Page");

        return View(new Supplier());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid supplier create request.");

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Please enter valid supplier information.";

            return View(supplier);
        }

        try
        {
            supplier.CreatedBy = 1;

            var id = await _supplierService.SaveAsync(supplier);

            _logger.LogInfo($"Supplier created successfully. SupplierId={id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Supplier created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Supplier create failed. SupplierId={supplier.Id}", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(supplier);
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        _logger.LogInfo($"Loading Supplier Edit. SupplierId={id}");

        var supplier = await _supplierService.GetByIdAsync(id);

        if (supplier == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Supplier not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Please enter valid supplier information.";

            return View(supplier);
        }

        try
        {
            supplier.ModifiedBy = 1;

            await _supplierService.SaveAsync(supplier);

            _logger.LogInfo($"Supplier updated successfully. SupplierId={supplier.Id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Supplier updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Supplier update failed. SupplierId={supplier.Id}",ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(supplier);
        }
    }

    #endregion

    #region Details

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo($"Loading Supplier Details. SupplierId={id}");

        var supplier = await _supplierService.GetByIdAsync(id);

        if (supplier == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Supplier not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(supplier);
    }

    #endregion

    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _supplierService.DeleteAsync(id, 1);

            if (result)
            {
                _logger.LogInfo($"Supplier deleted successfully. SupplierId={id}");

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Supplier deleted successfully.";
            }
            else
            {
                _logger.LogWarning($"Supplier delete failed. SupplierId={id}");

                TempData["AlertType"] = "error";
                TempData["AlertMessage"] = "Delete failed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Supplier delete failed. SupplierId={id}", ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}