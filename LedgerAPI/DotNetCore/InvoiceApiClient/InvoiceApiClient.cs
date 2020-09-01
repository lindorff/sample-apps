using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;

namespace InvoiceApiClient
{
    public class InvoiceApiClient
    {
        private string AccessTokenUrl { get; }
        private string ClientId { get; }
        private string ClientSecret { get; }
        private RestClient RestClient { get; }

        public InvoiceApiClient(string accessTokenUrl, string clientId, string clientSecret, string host)
        {
            AccessTokenUrl = accessTokenUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
            RestClient = new RestClient(new JsonDeserializer(), host);
        }

        public JsonArray GetInvoices(string resource)
        {
            var token = GetAccessToken().Result;
            var request = new RestRequest(resource, Method.GET, null, token);
            return RestClient.Call<JsonArray>(request);
        }

        private async Task<string> GetAccessToken()
        {
            var tokenClient = new HttpClient();
            var clientCredentialsResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = AccessTokenUrl,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = "scope"
            });
            var token =  clientCredentialsResponse.AccessToken;
            return token;
        }
    }
}
