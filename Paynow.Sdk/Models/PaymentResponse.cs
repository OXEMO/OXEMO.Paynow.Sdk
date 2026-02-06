using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class PaymentResponse
    {
        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}