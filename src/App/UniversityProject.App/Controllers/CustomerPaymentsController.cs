using Microsoft.AspNetCore.Mvc;
using UniversityProject.Application.Logging;
using UniversityProject.Application.Repositories;
using UniversityProject.Application.ViewModel;

namespace UniversityProject.App.Controllers;

public class CustomerPaymentsController(
    ICustomerPaymentRepository _paymentRepository,
    ICustomerRepository _customerRepository,
    IAppLogger<CustomerPaymentsController> _logger)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string search, int page = 1)
    {
        try
        {
            // Call your repository to execute the stored procedure
            var result = await _paymentRepository.GetListAsync(search, page, 10);

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalCount = result.TotalCount;

            return View(result.Items); // Passes IEnumerable<CustomerPaymentListViewModel> to the view
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load customer payments.", ex);
            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = "Failed to load payment history.";
            return View(new List<CustomerPaymentListViewModel>());
        }
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.Customers =
                await _customerRepository.GetDropdownAsync();

            var model = new CustomerPaymentViewModel
            {
                PaymentDate = DateTime.Now,
                PaymentMethod = "Cash",
                Invoices = new List<CustomerUnpaidInvoiceViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Customer payment create page failed.",
                ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }


    

    [HttpGet]
    public async Task<IActionResult> GetCustomerInvoices(
        long customerId)
    {
        if (customerId <= 0)
        {
            return Json(
                new List<CustomerUnpaidInvoiceViewModel>());
        }

        try
        {
            var invoices =
                await _paymentRepository
                    .GetCustomerUnpaidInvoices(customerId);

            return Json(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Failed loading unpaid invoices. CustomerId={customerId}",
                ex);

            return StatusCode(
                500,
                new
                {
                    message = ex.Message
                });
        }
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CustomerPaymentViewModel model)
    {
        try
        {


            if (model.CustomerId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.CustomerId),
                    "Please select customer.");
            }
            var selectedPayments =
                model.Invoices?
                    .Where(x => x.CollectionAmount > 0)
                    .ToList();
            if (selectedPayments == null ||
                selectedPayments.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter at least one collection amount.");
            }


     

            if (selectedPayments != null)
            {
                foreach (var invoice in selectedPayments)
                {
                    if (invoice.CollectionAmount >
                        invoice.RemainingDueAmount)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Collection amount for invoice " +
                            $"{invoice.InvoiceNo} cannot exceed " +
                            $"remaining due amount.");
                    }

                    if (invoice.CollectionAmount <= 0)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Invalid collection amount for invoice " +
                            $"{invoice.InvoiceNo}.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customers =
                    await _customerRepository
                        .GetDropdownAsync();

                return View(model);
            }
            model.Invoices = selectedPayments;




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
                    $"Customer payment saved. " +
                    $"CustomerId={model.CustomerId}, " +
                    $"Amount={model.TotalPayment}");

                TempData["AlertType"] = "success";

                TempData["AlertMessage"] =
                    "Customer payment saved successfully.";

                return RedirectToAction(nameof(Create));
            }


            TempData["AlertType"] = "error";

            TempData["AlertMessage"] =
                "Payment save failed.";

            ViewBag.Customers =
                await _customerRepository
                    .GetDropdownAsync();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Customer payment save failed. " +
                $"CustomerId={model.CustomerId}",
                ex);

            TempData["AlertType"] = "error";
            TempData["AlertMessage"] = ex.Message;

            ViewBag.Customers =
                await _customerRepository
                    .GetDropdownAsync();

            return View(model);
        }
    }
}