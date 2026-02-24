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
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("authorizationType")]
        public string? AuthorizationType { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("paymentMethodToken")]
        public string? PaymentMethodToken { get; set; }

        [JsonPropertyName("expirationDate")]
        public string? ExpirationDate { get; set; }

        [JsonPropertyName("savedInstruments")]
        public List<SavedInstrument>? SavedInstruments { get; set; }
    }

    public class SavedInstrument
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("expirationDate")]
        public string? ExpirationDate { get; set; }

        [JsonPropertyName("brand")]
        public string? Brand { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}