using Financial.Api.Infrastructure.Exceptions;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Financial.Api.Clients
{
    public static class RapidApiClient
    {
        public static async Task<IRestResponse> GetRequestAsync(string key, string host, string url, string value)
        {
            try
            {
                RestClient client = new RestClient(string.Format(url, value));
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-key", key);
                request.AddHeader("x-rapidapi-host", host);

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    throw new ClientUnsuccessfulResponseException(response.StatusCode.ToString());
                }

                return response;
            }
            catch (ClientUnsuccessfulResponseException ex)
            {
                throw new ClientUnsuccessfulResponseException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
