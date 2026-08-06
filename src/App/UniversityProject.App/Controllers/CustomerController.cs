using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.Services;
using UniversityProject.Core.Entities;

namespace UniversityProject.App.Controllers;

public class CustomerController(
    ICustomerRepository _customerService,
    IAppLogger<CustomerController> _logger) : Controller
{
    #region List

    public async Task<IActionResult> Index(string search, int page = 1)
    {
        _logger.LogInfo($"Loading Customer List. Search={search}, Page={page}");

        var result = await _customerService.GetListAsync(search, page, 10);

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
        _logger.LogInfo("Customer Create Page");

        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid customer create request.");

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Please enter valid customer information.";

            return View(customer);
        }

        try
        {
            customer.CreatedBy = 1;

            var id = await _customerService.SaveAsync(customer);

            _logger.LogInfo($"Customer Created. CustomerId={id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Customer created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( "Customer create failed.",ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(customer);
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        _logger.LogInfo($"Loading Customer Edit. CustomerId={id}");

        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Customer not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Invalid customer information.";

            return View(customer);
        }

        try
        {
            customer.ModifiedBy = 1;

            await _customerService.SaveAsync(customer);

            _logger.LogInfo($"Customer Updated. CustomerId={customer.Id}");

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Customer updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Customer Update Failed. CustomerId={customer.Id}",ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return View(customer);
        }
    }

    #endregion

    #region Details

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInfo($"Loading Customer Details. CustomerId={id}");

        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Customer not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    #endregion

    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _customerService.DeleteAsync(id, 1);

            if (result)
            {
                _logger.LogInfo($"Customer Deleted. CustomerId={id}");

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Customer deleted successfully.";
            }
            else
            {
                _logger.LogWarning($"Customer Delete Failed. CustomerId={id}");

                TempData["AlertType"] = "error";
                TempData["AlertMessage"] = "Delete failed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError( $"Customer Delete Exception. CustomerId={id}",ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}