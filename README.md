# Paynow .NET SDK

Nieoficjalne SDK dla bramki płatniczej Paynow (API v3).

Biblioteka udostępnia m.in.:

- tworzenie płatnoœci (`CreatePaymentAsync`)
- sprawdzanie statusu (`GetPaymentStatusAsync`)
- pobieranie szczegółów płatnoœci (`GetPaymentDetailsAsync`)
- zwroty (`CreateRefundAsync`)
- obsługę tokenizacji (pobieranie zapisanych instrumentów, płatność zapisaną kartą, usuwanie tokena)
- pobieranie klauzul RODO (`GetDataProcessingNoticesAsync`)

## Wymagania

- `netstandard2.1` (biblioteka) – do użycia w .NET 6/7/8 itp.
- Klucze z panelu Paynow: `ApiKey` oraz `SignatureKey`

## Instalacja

```bash
dotnet add package OXEMO.Paynow.Sdk
```

## Konfiguracja (DI) – ASP.NET Core (Razor Pages)

W `Program.cs`:

```csharp
using Paynow.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddPaynow(options =>
{
    options.ApiUrl = builder.Configuration["Paynow:ApiUrl"] ?? "https://api.sandbox.paynow.pl";
    options.ApiKey = builder.Configuration["Paynow:ApiKey"]!;
    options.SignatureKey = builder.Configuration["Paynow:SignatureKey"]!;
});

var app = builder.Build();
app.MapRazorPages();
app.Run();
```

Przykładowy `appsettings.json`:

```json
{
  "Paynow": {
    "ApiUrl": "https://api.sandbox.paynow.pl",
    "ApiKey": "<apiKey>",
    "SignatureKey": "<signatureKey>"
  }
}
```

Uwaga: klucze trzymaj w zmiennych środowiskowych / User Secrets, a nie w repo.

## Podstawowy flow płatności (redirect)

### 1) Utworzenie płatności

`amount` przekazywany jest w groszach (np. `100` = 1.00 PLN).

```csharp
using Paynow.Sdk;
using Paynow.Sdk.Models;

public class CheckoutModel : PageModel
{
    private readonly IPaynowClient _paynow;

    public CheckoutModel(IPaynowClient paynow) => _paynow = paynow;

    public async Task<IActionResult> OnPostPayAsync(int amountPln, string email)
    {
        var externalId = Guid.NewGuid().ToString();
        var continueUrl = Url.PageLink("/Paynow/Return");

        var request = new PaymentRequest
        {
            Amount = amountPln * 100,
            ExternalId = externalId,
            Description = $"Zamówienie {externalId}",
            ContinueUrl = continueUrl,
            Buyer = new Buyer
            {
                Email = email,
                Locale = "pl-PL"
            }
        };

        var response = await _paynow.CreatePaymentAsync(request);
        if (response?.RedirectUrl is null)
        {
            return BadRequest("Brak redirectUrl w odpowiedzi Paynow");
        }

        return Redirect(response.RedirectUrl);
    }
}
```

### 2) Obsługa powrotu (`continueUrl`)

Paynow dokleja do `continueUrl` parametr `paymentId`.

```csharp
using Paynow.Sdk;
using Paynow.Sdk.Models;

public class ReturnModel : PageModel
{
    private readonly IPaynowClient _paynow;
    public PaymentStatusResponse? Status { get; private set; }

    public ReturnModel(IPaynowClient paynow) => _paynow = paynow;

    public async Task OnGetAsync(string paymentId)
    {
        Status = await _paynow.GetPaymentStatusAsync(paymentId);
    }
}
```

## Pobieranie szczegółów płatnoœci (payment details)

```csharp
var details = await paynow.GetPaymentDetailsAsync(paymentId);
// details.PaymentMethodToken (jeśli API zwraca)
```

## Zwroty (Refund)

```csharp
var refund = await paynow.CreateRefundAsync(paymentId, new RefundRequest
{
    Amount = 100, // grosze
    Reason = "Zwrot testowy"
});
```

## Tokenizacja kart (zapis karty + płatność zapisaną kartą)

Poniższy opis odpowiada fragmentowi dokumentacji Paynow o tokenizacji.

### Zapis karty (tokenizacja podczas normalnej płatnoœci)

Aby Paynow udostępnił zapis karty i później pokazał zapisane instrumenty na paywallu, w `buyer` przekaż:

- `Buyer.ExternalId` – stały identyfikator kupującego (np. ID użytkownika z Twojej bazy)
- `Buyer.DeviceFingerprint` – fingerprint urządzenia

```csharp
var request = new PaymentRequest
{
    Amount = 10000,
    ExternalId = Guid.NewGuid().ToString(),
    Description = "Płatność za zamówienie",
    Buyer = new Buyer
    {
        ExternalId = "c0940962-706e-4e23-822a-97810744f1c5",
        DeviceFingerprint = "<fingerprint>",
        Email = "jan.kowalski@melements.pl"
    }
};

var response = await paynow.CreatePaymentAsync(request);
```

### Pobieranie zapisanych kart/instrumentów

SDK mapuje odpowiedź `GET /v3/payments/paymentmethods?externalBuyerId=...`.

```csharp
using Paynow.Sdk.Models;

var methods = await paynow.GetSavedPaymentMethodsAsync(externalBuyerId);

var cards = methods?
    .FirstOrDefault(m => m.Type == "CARD")?
    .PaymentMethods
    .SelectMany(pm => pm.SavedInstruments ?? new List<SavedInstrument>())
    .ToList();

var firstActiveToken = cards?.FirstOrDefault(c => c.Status == "ACTIVE")?.Token;
```

Token zapisanej karty jest w `SavedInstrument.Token`.

### Płatność zapisaną kartą (White Label)

Po wybraniu karty przez kupującego wyślij do Paynow:

- `PaymentRequest.PaymentMethodId` (np. 2002)
- `PaymentRequest.PaymentMethodToken` (token instrumentu)

```csharp
var request = new PaymentRequest
{
    Amount = 45671,
    ExternalId = Guid.NewGuid().ToString(),
    Description = "Płatność zapisaną kartą",
    PaymentMethodId = "2002",
    PaymentMethodToken = firstActiveToken,
    Buyer = new Buyer
    {
        ExternalId = externalBuyerId,
        Email = email
    }
};

var response = await paynow.CreatePaymentAsync(request);
return Redirect(response!.RedirectUrl);
```

Uwaga: `paymentMethodId` w API jest liczbą – w SDK `PaymentRequest.PaymentMethodId` jest stringiem. Przekazuj wartość w postaci tekstu, np. `"2002"`.

### Usuwanie zapisanej karty

```csharp
await paynow.RemoveSavedPaymentMethodAsync(token, externalBuyerId);
```

## Klauzule RODO (wymagane dla White Label)

```csharp
var notices = await paynow.GetDataProcessingNoticesAsync("pl-PL");
```

## Powiadomienia (webhook) i weryfikacja podpisu

Paynow wysyła statusy na `notification-url` jako `POST` z nagłówkiem `Signature`.

Do weryfikacji podpisu użyj `VerifySignature(...)`.

```csharp
using System.Text;

Request.EnableBuffering();
using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
var body = await reader.ReadToEndAsync();
Request.Body.Position = 0;

var signature = Request.Headers["Signature"].ToString();

var ok = paynow.VerifySignature(
    signature,
    body,
    new Dictionary<string, string>()
);

if (!ok) return Unauthorized();
```

Model pomocniczy (opcjonalny) do deserializacji notyfikacji: `Paynow.Sdk.Models.PaymentNotification`.

## Obsługa błędów

W przypadku błędów API rzucany jest `PaynowException`.

```csharp
try
{
    var status = await paynow.GetPaymentStatusAsync(paymentId);
}
catch (PaynowException ex)
{
    // ex.StatusCode
    // ex.Errors (lista szczegółów)
}

```
