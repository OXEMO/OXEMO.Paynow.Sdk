using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Paynow.Sdk.Models;
using Paynow.Sdk.Utilities;

namespace Paynow.Sdk
{
    public class PaynowClient : IPaynowClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaynowOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public PaynowClient(HttpClient httpClient, PaynowOptions options)
        {
            _httpClient = httpClient;
            _options = options;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_options.ApiUrl);
            }
        }

        public async Task<PaymentResponse?> CreatePaymentAsync(PaymentRequest request, string? idempotencyKey = null)
        {
            var jsonBody = JsonSerializer.Serialize(request, _jsonOptions);
            return await SendRequestAsync<PaymentResponse>(HttpMethod.Post, "/v3/payments", jsonBody, idempotencyKey);
        }

        public async Task<PaymentStatusResponse?> GetPaymentStatusAsync(string paymentId)
        {
            return await SendRequestAsync<PaymentStatusResponse>(HttpMethod.Get, $"/v3/payments/{paymentId}/status", "");
        }

        public async Task<RefundResponse?> CreateRefundAsync(string paymentId, RefundRequest request, string? idempotencyKey = null)
        {
            var jsonBody = JsonSerializer.Serialize(request, _jsonOptions);
            return await SendRequestAsync<RefundResponse>(HttpMethod.Post, $"/v3/payments/{paymentId}/refunds", jsonBody, idempotencyKey);
        }

        public bool VerifySignature(string signatureFromHeader, string bodyString, IDictionary<string, string> headers)
        {
            try
            {
                var calculatedSignature = SignatureCalculator.Calculate(
                    _options.SignatureKey,
                    bodyString,
                    headers,
                    new Dictionary<string, string>()
                );

                return calculatedSignature == signatureFromHeader;
            }
            catch
            {
                return false;
            }
        }

        private async Task<T?> SendRequestAsync<T>(HttpMethod method, string path, string jsonBody, string? idempotencyKey = null)
            where T : class
        {

            var request = new HttpRequestMessage(method, path);
            idempotencyKey ??= Guid.NewGuid().ToString();


            var headersForSignature = new Dictionary<string, string>
            {
                { "Api-Key", _options.ApiKey },
                { "Idempotency-Key", idempotencyKey }
            };

            var queryParams = ExtractQueryParams(path);

            var signature = SignatureCalculator.Calculate(
                _options.SignatureKey,
                jsonBody,
                headersForSignature,
                queryParams
            );

            request.Headers.Add("Api-Key", _options.ApiKey);
            request.Headers.Add("Idempotency-Key", idempotencyKey);
            request.Headers.Add("Signature", signature);
            request.Headers.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Paynow API Error: {response.StatusCode}. Details: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            return result;
        }

        private Dictionary<string, string> ExtractQueryParams(string path)
        {
            var queryParams = new Dictionary<string, string>();
            if (path.Contains("?"))
            {
                var query = path.Split('?')[1];
                foreach (var part in query.Split('&'))
                {
                    var kv = part.Split('=');
                    if (kv.Length == 2) queryParams[kv[0]] = kv[1];
                }
            }
            return queryParams;
        }
    }
}