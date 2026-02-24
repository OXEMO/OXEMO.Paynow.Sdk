using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class PaymentDetailsResponse
    {
        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        [JsonPropertyName("paymentMethodToken")]
        public string? PaymentMethodToken { get; set; }

        [JsonPropertyName("paymentMethodId")]
        public string? PaymentMethodId { get; set; }
    }
}
