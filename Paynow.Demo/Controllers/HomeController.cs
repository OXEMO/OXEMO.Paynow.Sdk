using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Paynow.Demo.Models;
using Paynow.Sdk;
using Paynow.Sdk.Models;

namespace Paynow.Demo.Controllers;

public class HomeController : Controller
{
    private readonly IPaynowClient _paynowClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IPaynowClient paynowClient, ILogger<HomeController> logger)
    {
        _paynowClient = paynowClient;
        _logger = logger;
    }

    // 1. Wyœwietla formularz p³atnoœci
    public IActionResult Index()
    {
        return View();
    }

    // 2. Tworzy p³atnoœæ i przekierowuje do Paynow
    [HttpPost]
    public async Task<IActionResult> CreatePayment(int amountPln, string email)
    {
        try
        {
            var externalId = Guid.NewGuid().ToString(); // Unikalny ID zamówienia

            // Budujemy URL powrotu (na localhost)
            var continueUrl = Url.Action("PaymentReturn", "Home", null, Request.Scheme);

            var request = new PaymentRequest
            {
                Amount = amountPln * 100, // Konwersja na grosze!
                ExternalId = externalId,
                Description = $"Zamówienie testowe {externalId.Substring(0, 8)}",
                Buyer = new Buyer { Email = email, FirstName = "Jan", LastName = "Testowy", Locale = "pl-PL" },
                ContinueUrl = continueUrl
            };

            var response = await _paynowClient.CreatePaymentAsync(request);

            if (response != null && !string.IsNullOrEmpty(response.RedirectUrl))
            {
                // Zapisujemy ID p³atnoœci w TempData/Sesji, ¿eby mieæ je po powrocie
                // W prawdziwej aplikacji zapisa³byœ to w bazie danych przy zamówieniu.
                return Redirect(response.RedirectUrl);
            }

            return View("Error", new ErrorViewModel { RequestId = "Brak RedirectUrl w odpowiedzi Paynow" });
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }
    }

    // 3. U¿ytkownik wraca tutaj po p³atnoœci (ContinueUrl)
    // Paynow dokleja ?paymentId=... do adresu
    [HttpGet]
    public async Task<IActionResult> PaymentReturn(string paymentId)
    {
        if (string.IsNullOrEmpty(paymentId))
        {
            return RedirectToAction("Index");
        }

        try
        {
            // Sprawdzamy status p³atnoœci
            var status = await _paynowClient.GetPaymentStatusAsync(paymentId);
            return View("Status", status);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }
    }

    // 4. Testowanie zwrotu (Refund)
    [HttpPost]
    public async Task<IActionResult> Refund(string paymentId, int amount)
    {
        try
        {
            var request = new RefundRequest
            {
                Amount = amount, // Kwota w groszach
                Reason = "Testowy zwrot z demo"
            };

            var response = await _paynowClient.CreateRefundAsync(paymentId, request);

            ViewBag.Message = $"Zwrot zlecony! ID Zwrotu: {response.RefundId}, Status: {response.Status}";

            // Ponownie pobieramy status p³atnoœci, by wyœwietliæ widok
            var status = await _paynowClient.GetPaymentStatusAsync(paymentId);
            return View("Status", status);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel { RequestId = ex.Message });
        }
    }
}