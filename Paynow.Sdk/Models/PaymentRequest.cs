using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class PaymentRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; } // Kwota w groszach

        [JsonPropertyName("externalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("buyer")]
        public Buyer Buyer { get; set; }

        [JsonPropertyName("continueUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContinueUrl { get; set; }

        [JsonPropertyName("paymentMethodId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PaymentMethodId { get; set; }
    }
}