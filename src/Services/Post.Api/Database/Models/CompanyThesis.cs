namespace Post.Api.Database.Models
{
    /// <summary>
    /// This section is used to answer a fundamental question: why should someone invest in this company?
    /// Company thesis section is divided into 5 subsections, each of which can be provided (or skipped) by the user:
    /// * Return On Investment (ROI)
    /// * Company Leadership
    /// * Future Advantage
    /// * Growth
    /// * Competitive Advantage
    /// </summary>
    public class CompanyThesis
    {
        public TextDescription RoiDescription { get; set; }
        public TextDescription LeadershipDescription { get; set; }
        public TextDescription FutureAdvantageDescription { get; set; }
        public TextDescription GrowthDescription { get; set; }
        public TextDescription CompetitiveAdvantageDescription { get; set; }
    }
}
