using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace WebCall
{
    public class WebCallService : IWebCallService
    {
        public TResponse Invoke<TResponse>(Uri endpointUri, Method method = Method.POST, string contentType = "application/x-www-form-urlencoded", Dictionary<WebCallHeaderType, string> customHeaders = null)
            where TResponse : new()
        {
            return Invoke<string, TResponse>(null, endpointUri, method, contentType, customHeaders);
        }

        public TResponse Invoke<TRequest, TResponse>(TRequest request, Uri endpointUri, Method method = Method.POST, string contentType = "application/x-www-form-urlencoded", Dictionary<WebCallHeaderType, string> customHeaders = null)
            where TResponse : new()
        {
            // Use TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(endpointUri);

            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var restRequest = new RestRequest(method);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = NewtonsoftJsonSerializer.Default;

            restRequest.AddHeader("content-type", contentType);

            customHeaders?.ToList().ForEach(h =>
            {
                var headerName = Enum.GetName(typeof(WebCallHeaderType), h.Key);
                var headerValue = h.Value;

                if (h.Key == WebCallHeaderType.Authorization)
                {
                    headerValue = $"Bearer {headerValue}";
                }

                restRequest.AddHeader(headerName, headerValue);
            });

            if (request != null)
            {
                if (contentType == "application/json" ||
                    contentType == "text/json" ||
                    contentType == "text/x-json" ||
                    contentType == "text/javascript" ||
                    contentType == "*+json")
                {
                    restRequest.AddBody(request);
                }
                else
                {
                    restRequest.AddObject(request);
                }
            }

            IRestResponse<TResponse> response = client.Execute<TResponse>(restRequest);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ApplicationException(response.Content);
            }

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }

            return response.Data;
        }
    }
}
