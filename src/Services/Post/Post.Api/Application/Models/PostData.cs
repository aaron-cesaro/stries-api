using Post.Api.Database.Models;

namespace Post.Api.Application.Models
{
    public class PostData
    {
        public CompanyDescription CompanyDescription { get; set; }
        public CompanyThesis CompanyThesis { get; set; }
        public CompanyValuation CompanyValuation { get; set; }
        public CompanyFinancials CompanyFinancials { get; set; }
        public CompanyOther CompanyOther { get; set; }
    }
}
