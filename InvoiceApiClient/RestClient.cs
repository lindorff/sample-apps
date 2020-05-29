using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using RestSharp;
using RestSharp.Deserializers;

namespace InvoiceApiClient
{
    sealed class RestClient : RestSharp.RestClient
    {

        public RestClient(IDeserializer serializer, string baseUrl)
        {
            AddHandler("application/json", () => serializer);
            AddHandler("text/json", () => serializer);
            AddHandler("text/x-json", () => serializer);
            BaseUrl = new Uri(baseUrl);
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            var response = base.Execute(request);
            return response;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            var response = base.Execute<T>(request);
            return response;
        }
        
        public T Call<T>(IRestRequest request, HttpStatusCode[] additionalAcceptedStatusCodes) where T : new()
        {
            var response = Execute<T>(request);
            if (response.StatusCode == HttpStatusCode.OK || 
                additionalAcceptedStatusCodes!=null && additionalAcceptedStatusCodes.Contains(response.StatusCode))
            {
                return response.Data;
            }
            LogError(BaseUrl, request, response);
            throw new Exception(GetRequestInfo(BaseUrl, request, response));
        }

        public T Call<T>(IRestRequest request) where T : new()
        {
            return Call<T>(request, null);
        }

        public bool Call(IRestRequest request, HttpStatusCode[] additionalAcceptedStatusCodes)
        {
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                additionalAcceptedStatusCodes != null && additionalAcceptedStatusCodes.Contains(response.StatusCode))
            {
                return true;
            }
            LogError(BaseUrl, request, response);
            throw new Exception(GetRequestInfo(BaseUrl, request, response));
        }

        public bool Call(IRestRequest request)
        {
            return Call(request, null);
        }

        private static void LogError(Uri baseUrl, IRestRequest request, IRestResponse response)
        {
            var info = GetRequestInfo(baseUrl, request, response);

            //Get the values of the parameters passed to the API
            var parameters = string.Join(", ", request.Parameters.Select(x => x.Name.ToString() + "=" + (x.Value ?? "NULL")).ToArray());

            info += ", parameters: " + parameters + ", and content: " + response.Content;

            //Acquire the actual exception
            var ex = response.ErrorException ?? new Exception(info);

            Trace.TraceError(ex.Message);
        }

        private static string GetRequestInfo(Uri baseUrl, IRestRequest request, IRestResponse response)
        {
            //Set up the information message with the URL, the status code, and the parameters.
            var info = "Request to " + baseUrl.AbsoluteUri + request.Resource + " failed with status code " + response.StatusCode;
            return info;
        }
    }
}
