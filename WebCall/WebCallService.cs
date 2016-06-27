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
            var client = new RestClient(endpointUri);
            var restRequest = new RestRequest(method);
            restRequest.AddHeader("content-type", contentType);

            customHeaders?.ToList().ForEach(h =>
            {
                var headerName = Enum.GetName(typeof (WebCallHeaderType), h.Key);
                var headerValue = h.Value;

                if (h.Key == WebCallHeaderType.Authorization)
                {
                    headerValue = $"Bearer {headerValue}";
                }

                restRequest.AddHeader(headerName, headerValue);
            });
            
            if (request != null)
            {
                restRequest.AddObject(request);
            }

            IRestResponse<TResponse> response = client.Execute<TResponse>(restRequest);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ApplicationException(response.StatusDescription);
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
