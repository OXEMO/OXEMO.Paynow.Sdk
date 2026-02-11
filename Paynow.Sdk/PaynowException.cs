using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Paynow.Sdk
{
    public class PaynowException : Exception
    {
        public int StatusCode { get; }
        public List<PaynowErrorDetail> Errors { get; }

        public PaynowException(int statusCode, List<PaynowErrorDetail> errors, string rawResponse)
            : base($"Paynow API Error {statusCode}: {rawResponse}")
        {
            StatusCode = statusCode;
            Errors = errors ?? new List<PaynowErrorDetail>();
        }
    }

    public class PaynowErrorDetail
    {
        [JsonPropertyName("errorType")]
        public string ErrorType { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    // Klasa pomocnicza do deserializacji błędu z API
    internal class PaynowErrorResponse
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errors")]
        public List<PaynowErrorDetail> Errors { get; set; }
    }
}