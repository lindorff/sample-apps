using RestSharp;

namespace InvoiceApiClient
{
    class RestRequest : RestSharp.RestRequest
    {
        public RestRequest(string resource, Method method, object jsonDataObject, string token) : base(resource, method)
        {
            if (jsonDataObject != null)
            {
                RequestFormat = DataFormat.Json;
                AddJsonBody(jsonDataObject);
            }
            AddHeader("Authorization", "Bearer " + token);
        }
    }
}
