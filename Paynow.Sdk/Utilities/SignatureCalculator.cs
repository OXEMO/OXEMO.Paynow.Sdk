using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Paynow.Sdk.Utilities
{
    public static class SignatureCalculator
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, 
            WriteIndented = false
        };
        public static string Calculate(
            string signatureKey,
            string bodyString,
            IDictionary<string, string> headers,
            IDictionary<string, string> queryParams)
        {

            var allowedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Api-Key",
                "Idempotency-Key"
            };

            var filteredHeaders = headers
                .Where(h => allowedHeaders.Contains(h.Key))
                .ToDictionary(k => k.Key, v => v.Value);

            var sortedHeaders = new SortedDictionary<string, string>(filteredHeaders, StringComparer.Ordinal);


            var sortedParams = new SortedDictionary<string, string[]>(StringComparer.Ordinal);
            if (queryParams != null)
            {
                foreach (var kvp in queryParams)
                {
                    sortedParams.Add(kvp.Key, new[] { kvp.Value });
                }
            }

            object bodyContent;
            if (string.IsNullOrEmpty(bodyString))
            {
                bodyContent = new { };
            }
            else
            {
                bodyContent = bodyString;
            }

            var payloadStructure = new
            {
                headers = sortedHeaders,
                parameters = sortedParams,
                body = bodyContent
            };

            var payloadToHash = JsonSerializer.Serialize(payloadStructure, _serializerOptions);


            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signatureKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadToHash));

            var signature = Convert.ToBase64String(hashBytes);

            return signature;
        }
    }
}