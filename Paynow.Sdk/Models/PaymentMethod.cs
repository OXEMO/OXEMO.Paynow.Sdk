using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class PaymentMethodsResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("paymentMethods")]
        public List<PaymentMethod> PaymentMethods { get; set; }
    }

    public class PaymentMethod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("paymentMethodToken")]
        public string PaymentMethodToken { get; set; }

        [JsonPropertyName("expirationDate")]
        public string ExpirationDate { get; set; }
    }
}