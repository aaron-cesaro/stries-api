using Financial.Api.Application.Models;
using Financial.Api.Clients;
using Financial.Api.Infrastructure.Exceptions;
using Financial.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Financial.Api.Managers
{
    public class FinancialManager : IFinancialManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly string _host;

        public FinancialManager(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _key = _configuration.GetValue<string>("RapidApiClient:Key");
            _host = _configuration.GetValue<string>("RapidApiClient:Host");
        }

        public async Task<List<CompanySearchResult>> SearchCompaniesAsync(string searchQuery)
        {
            var url = _configuration.GetValue<string>("RapidApiClient:AutoCompleteUrl");

            try
            {
                var response = await RapidApiClient.GetRequestAsync(_key, _host, url, searchQuery);

                var companySearchResultList = new List<CompanySearchResult>();

                var searchResponse = JsonDocument.Parse(response.Content);

                var companyCountResult = int.Parse(searchResponse.RootElement.GetProperty("count").ToString());

                if (companyCountResult != 0)
                {
                    var CompanySearchResultListJsonObject = searchResponse.RootElement.GetProperty("quotes").EnumerateArray();

                    foreach (var company in CompanySearchResultListJsonObject)
                    {
                        var searchedCompany = JsonConvert.DeserializeObject<CompanySearchResult>(company.ToString());
                        companySearchResultList.Add(searchedCompany);
                    }
                }

                return companySearchResultList;
            }
            catch (ClientUnsuccessfulResponseException ex)
            {
                Log.Error(ex, ex.Message, $"Api client unsuccessful response");
                throw new ClientUnsuccessfulResponseException(ex, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Api client error");
                throw new Exception(ex.Message);
            }
        }

        public Task<CompanyDetailsResponse> GetCompanyDetailsAsync(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}
