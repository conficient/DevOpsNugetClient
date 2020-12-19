using RestSharp;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace DevOpsNugetClient
{
    /// <summary>
    /// Exception raised for REST errors
    /// </summary>
    [Serializable]
    public class RestException : Exception
    {
        /// <summary>
        /// Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        public RestException()
        {
        }

        public RestException(IRestResponse result) : base(result.StatusDescription)
        {
            StatusCode = result.StatusCode;
        }

        public RestException(HttpStatusCode code, string description) : base(description)
        {
            StatusCode = code;
        }
    }
}