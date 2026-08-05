using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Extensions;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Core.Entities;

namespace UniversityProject.App.Controllers;

public class ProductsController(
    IProductRepository _productService,
    IAppLogger<ProductsController> _logger) : Controller
{
    public async Task<IActionResult> Index(string search, int page = 1)
    {
        try
        {
            _logger.LogInfo($"Loading Product List. Search={search}, Page={page}");

            var result = await _productService.GetListAsync(search, page, 10);

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalCount = result.TotalCount;

            return View(result.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while loading Product List.",ex);

            TempData["AlertMessage"] = "Unable to load product list.";
            TempData["AlertType"] = "error";

            return View(new List<Product>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        _logger.LogInfo("Product Create page accessed.");
        Product product = new Product();
        product.Barcode = BarcodeGenerator.Generate();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid Product Create request.");

                TempData["AlertMessage"] = "Please fill all required fields.";
                TempData["AlertType"] = "error";

                return View(model);
            }

            model.CreatedBy = 1; // Replace with LoggedIn User Id

            var id = await _productService.AddAsync(model);

            _logger.LogInfo($"Product created successfully. ProductId={id}, ProductName={model.ProductName}");

            TempData["AlertMessage"] = "Product created successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Error while creating Product. ProductName={model.ProductName}",ex);

            TempData["AlertMessage"] = "An error occurred while creating the product.";
            TempData["AlertType"] = "error";

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            _logger.LogInfo($"Loading Product Edit page. ProductId={id}");

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Product not found. ProductId={id}");

                TempData["AlertMessage"] = "Product not found.";
                TempData["AlertType"] = "warning";

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Error loading Product Edit page. ProductId={id}", ex);

            TempData["AlertMessage"] = "An error occurred.";
            TempData["AlertType"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid Product Update request. ProductId={model.Id}");

                TempData["AlertMessage"] = "Invalid product information.";
                TempData["AlertType"] = "error";

                return View(model);
            }

            model.ModifiedBy = 1; // Replace with LoggedIn User Id

            var result = await _productService.UpdateAsync(model);

            if (!result)
            {
                _logger.LogWarning($"Product update failed. ProductId={model.Id}");

                TempData["AlertMessage"] = "Update failed.";
                TempData["AlertType"] = "error";

                return View(model);
            }

            _logger.LogInfo($"Product updated successfully. ProductId={model.Id}");

            TempData["AlertMessage"] = "Product updated successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Error while updating Product. ProductId={model.Id}",ex);

            TempData["AlertMessage"] = "An error occurred while updating the product.";
            TempData["AlertType"] = "error";

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            _logger.LogInfo($"Deleting Product. ProductId={id}");

            var result = await _productService.DeleteAsync(id, 1); // Replace 1 with LoggedIn User Id

            if (!result)
            {
                _logger.LogWarning($"Delete failed. ProductId={id}");

                TempData["AlertMessage"] = "Delete failed.";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            _logger.LogInfo($"Product deleted successfully. ProductId={id}");

            TempData["AlertMessage"] = "Product deleted successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Error while deleting Product. ProductId={id}", ex);

            TempData["AlertMessage"] = "An error occurred while deleting the product.";
            TempData["AlertType"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        try
        {
            _logger.LogInfo($"Loading Product Details. ProductId={id}");

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Product not found. ProductId={id}");

                TempData["AlertMessage"] = "Product not found.";
                TempData["AlertType"] = "warning";

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Error while loading Product Details. ProductId={id}",ex);

            TempData["AlertMessage"] = "Something went wrong.";
            TempData["AlertType"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }
}