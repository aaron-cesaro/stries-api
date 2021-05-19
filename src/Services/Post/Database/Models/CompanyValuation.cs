using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Database.Models
{
    /// <summary>
    /// Representation of the current company valuation based on one or more of the following 6 methods:
    /// * Market Cap Valuation
    /// * Times Revenue Method Valuation
    /// * Earning Multiplier Valuation
    /// * Discount Cash Flow Method Valuation
    /// * Book Value Valuation
    /// * Liquidation Value Valuation
    /// </summary>
    public class CompanyValuation
    {
        public TextDescription MarketCapValuationDescription { get; set; }
        public TextDescription TimesRevenueMethodValuationDescription { get; set; }
        public TextDescription EarningMultiplierValuationDescription { get; set; }
        public TextDescription DcfMethodValuationDescription { get; set; }
        public TextDescription BookValueValuationDescription { get; set; }
        public TextDescription LiquidationValueValuationDescription { get; set; }
    }
}
