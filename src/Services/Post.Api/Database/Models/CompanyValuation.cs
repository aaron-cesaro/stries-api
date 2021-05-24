namespace Post.Api.Database.Models
{
    /// <summary>
    /// Representation of the current company valuation based on one or more of the following 3 methods:
    /// * Market Cap Valuation
    /// * Discount Cash Flow Method Valuation
    /// * Book Value Valuation
    /// </summary>
    public class CompanyValuation
    {
        public TextDescription MarketCapValuationDescription { get; set; }
        public TextDescription DcfMethodValuationDescription { get; set; }
        public TextDescription BookValueValuationDescription { get; set; }
    }
}
