using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Paynow.Sdk.Models
{
    // Wrapper, bo API zwraca { "notices": [ ... ] }
    internal class NoticeResponseWrapper
    {
        [JsonPropertyName("notices")]
        public List<Notice> Notices { get; set; }
    }

    public class Notice
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }
    }
}