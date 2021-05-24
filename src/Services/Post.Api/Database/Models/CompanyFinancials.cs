using System.ComponentModel.DataAnnotations;

namespace Post.Api.Database.Models
{
    /// <summary>
    /// Representation of the current and past financial status of a company.
    /// The model consists of 3 major sections:
    /// * Company Income statement
    /// * Company Balance Sheet
    /// * Company Cash Flow
    /// </summary>
    public class CompanyFinancials
    {
        [Required(ErrorMessage = "Income Statement field must be provided")]
        public IncomeStatementSection IncomeStatement { get; set; }

        [Required(ErrorMessage = "Balance Sheet field must be provided")]
        public BalanceSheetSection BalanceSheet { get; set; }

        [Required(ErrorMessage = "Cash Flow field must be provided")]
        public CashFlowSection CashFlow { get; set; }
    }
}
