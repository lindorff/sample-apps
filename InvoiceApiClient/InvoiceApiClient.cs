using System.Net;
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

        public JsonArray GetInvoices(string customerId)
        {
            string resource = $"fin/qa/invoice/customers/{customerId}/invoices";
            var request = new RestRequest(resource, Method.GET, null, GetToken());
            return RestClient.Call<JsonArray>(request);
        }

        private string GetToken()
        {
            var client = new WebClient();
            var postData = $"grant_type=client_credentials&client_id={ClientId}&client_secret={ClientSecret}";
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var responseJson = client.UploadData(
                AccessTokenUrl,
                "POST",
                System.Text.Encoding.ASCII.GetBytes(postData)
            );
            var joResponse = JObject.Parse(System.Text.Encoding.Default.GetString(responseJson));
            var token = joResponse["access_token"].ToString();
            return token;
        }
    }
}
