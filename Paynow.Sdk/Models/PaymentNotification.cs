using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class PaymentNotification
    {
        [JsonPropertyName("eventType")]
        public string? EventType { get; set; }

        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("paymentMethodToken")]
        public string? PaymentMethodToken { get; set; }
    }
}
