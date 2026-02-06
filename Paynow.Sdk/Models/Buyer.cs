using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class Buyer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("firstName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LastName { get; set; }

        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; } // np. "pl-PL"
    }
}