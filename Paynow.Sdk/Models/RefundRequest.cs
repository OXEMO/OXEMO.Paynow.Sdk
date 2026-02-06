using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class RefundRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("reason")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Reason { get; set; }
    }
}