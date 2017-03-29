using System;
using System.Collections.Generic;
using RestSharp;

namespace WebCall.Interfaces
{
    public interface IWebCallService
    {
        TResponse Invoke<TResponse>(Uri endpointUri, Method method = Method.POST, string contentType = "application/x-www-form-urlencoded", Dictionary<WebCallHeaderType, string> customHeaders = null)
            where TResponse : new();

        TResponse Invoke<TRequest, TResponse>(TRequest request, Uri endpointUri,
            Method method = Method.POST,
            string contentType = "application/x-www-form-urlencoded", Dictionary<WebCallHeaderType, string> customHeaders = null)
            where TResponse : new();
    }
}