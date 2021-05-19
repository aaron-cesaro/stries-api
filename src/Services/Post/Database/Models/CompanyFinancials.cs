namespace Post.Database.Models
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
        public IncomeStatement IncomeStatement { get; set; }
        public BalanceSheet BalanceSheet { get; set; }
        public CashFlow CashFlow { get; set; }
    }
}
