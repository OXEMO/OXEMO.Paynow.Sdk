using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    public class RefundResponse
    {
        [JsonPropertyName("refundId")]
        public string RefundId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}