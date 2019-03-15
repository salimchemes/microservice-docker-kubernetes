using System;
using System.Runtime.Serialization;

namespace VillaSport.MicroService.Proxies
{
    [Serializable]
    internal class RestHttpProxyException : Exception
    {
        private object displayMessage;
        private object statusCode;
        private string v1;
        private object exceptionMessage;
        private string v2;
        private object p;

        public RestHttpProxyException()
        {
        }

        public RestHttpProxyException(string message) : base(message)
        {
        }

        public RestHttpProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RestHttpProxyException(object displayMessage, object statusCode, string v1, object exceptionMessage, string v2, object p)
        {
            this.displayMessage = displayMessage;
            this.statusCode = statusCode;
            this.v1 = v1;
            this.exceptionMessage = exceptionMessage;
            this.v2 = v2;
            this.p = p;
        }

        protected RestHttpProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}