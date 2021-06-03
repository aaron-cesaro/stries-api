using Financial.Api.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financial.Api.Interfaces
{
    public interface IFinancialManager
    {
        Task<List<CompanySearchResult>> SearchCompaniesAsync(string searchQuery);
        Task<CompanyDetailsResponse> GetCompanyDetailsAsync(string symbol);
    }
}
