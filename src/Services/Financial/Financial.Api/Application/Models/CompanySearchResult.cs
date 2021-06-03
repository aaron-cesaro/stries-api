using System.Text.Json.Serialization;

namespace Financial.Api.Application.Models
{
    public class CompanySearchResult
    {
        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }

        [JsonPropertyName("shortname")]
        public string Shortname { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("quoteType")]
        public string QuoteType { get; set; }
    }
}
