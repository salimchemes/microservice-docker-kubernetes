using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using VillaSport.MicroService.Exceptions;
using Polly;

namespace VillaSport.MicroService.Proxies
{
    /// <summary>
    /// Generic reusable class to facilitate calls to REST web services
    /// </summary>
    public class RestHttpProxy : IRestHttpProxy
    {
        #region Members

        /// <summary>
        /// Default Attempts: 3
        /// </summary>
        public int RetryAttempts { get; set; } = 3;
        private readonly bool _log = false;
        private readonly List<string> _logDataFormats = new List<string>();
        //private TelemetryClient tc = new TelemetryClient();
        private static HttpClient client;

        private Dictionary<string, string> RecurrentRequestHeaders { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public RestHttpProxy()
        {
            if (client != null)
                return;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        public RestHttpProxy(bool log, List<string> logDataFormats) : this()
        {
            _log = log;
            _logDataFormats = logDataFormats;
        }

        public RestHttpProxy(bool log, List<string> logDataFormats, Dictionary<string, string> recurrentHeaders) : this(log, logDataFormats)
        {
            RecurrentRequestHeaders = recurrentHeaders;
        }

        #endregion

        #region Methods

        #region Gets
        public T GetWebRequest<T>(Uri uri)
        {
            try
            {
                return GetWebRequestAsync<T>(uri).Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        /// <summary>
        /// Perform async Http GETs with generics
        /// </summary>        
        /// <param name="uri"></param>        
        /// <returns></returns>
        public async Task<T> GetWebRequestAsync<T>(Uri uri)
        {
            string jsonString = null;

            using (var response = await SendWithRetryAsync(() => BuildRequest(HttpMethod.Get, uri)).ConfigureAwait(false))
            {
                jsonString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                var apiEx = JsonConvert.DeserializeObject<ApiExceptionResponse>(jsonString);
                throw new RestHttpProxyException(apiEx.DisplayMessage, response.StatusCode, "GET", apiEx.ExceptionMessage, uri.ToString(), null);
            }
        }
        #endregion

        #region Posts         

        public T PostWebRequest<T>(Uri uri, object requestObject, Dictionary<string, string> headers = null) where T : new()
        {
            try
            {
                return PostWebRequestAsync<T>(uri, requestObject, headers).Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        public async Task<T> PostWebRequestAsync<T>(Uri uri, object request, Dictionary<string, string> headers = null) where T : new()
        {
            using (var response = await SendWithRetryAsync(() => BuildRequest(HttpMethod.Post, uri, request, headers)).ConfigureAwait(false))
            {
                var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                var message = jsonString;
                try
                {
                    var apiEx = JsonConvert.DeserializeObject<ApiExceptionResponse>(jsonString);
                    message = apiEx.DisplayMessage;
                }
                catch
                { }
                throw new RestHttpProxyException(message, response.StatusCode, "POST", jsonString, uri.ToString(), null);
            }
        }

        #endregion

        public void SetHeader(string key, string value)
        {

            if (RecurrentRequestHeaders == null)
                return;

            if (RecurrentRequestHeaders.ContainsKey(key))
                RecurrentRequestHeaders[key] = value;
            else
                RecurrentRequestHeaders.Add(key, value);
        }

        private bool IsTransientSocketException(Exception ex)
        {
            var isTransient = false;

            var sex = ex.GetBaseException() as SocketException;
            if (sex != null)
            {
                isTransient = sex.Message.Contains("actively refused") || sex.Message.Contains("forcibly closed");
                var exceptionProperties = new Dictionary<string, string>
                {
                    { "IsTransient", isTransient.ToString() },
                    { "SocketExceptionErrorCode", sex.SocketErrorCode.ToString() }
                };
                //tc.TrackException(sex, exceptionProperties);
            }

            return isTransient;
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, Uri uri, object request = null, Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(method, uri);

            if (request != null)
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            AddRecurrentHeaders(requestMessage);

            if (headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Remove(header.Key); // override existings keys such as Authorization & User-Agent
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            return requestMessage;
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(Func<HttpRequestMessage> requestBuilder)
        {
            HttpResponseMessage httpResponseMessage = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // retry immediately on specific SocketExceptions up to 3 times
            var pollyPolicy = Policy.Handle<Exception>(IsTransientSocketException)
            .RetryAsync(RetryAttempts, (exception, retryCount, context) =>
            {
                var uri = context["Uri"] as string;
                LogRetryException(exception, retryCount, stopwatch, uri);
            });

            var requestUri = string.Empty;
            var requestMethod = string.Empty;

            await pollyPolicy.ExecuteAsync(async (context) =>
            {
                using (var request = requestBuilder.Invoke())
                {
                    context["Uri"] = requestUri = request.RequestUri.AbsoluteUri;
                    requestMethod = request.Method.Method;

                    // appication insights will log API calls as dependencies
                    httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                }
            }, new Context("HttpProxy")).ConfigureAwait(false);

            stopwatch.Stop();

            if (httpResponseMessage == null)
                throw new RestHttpProxyException("System is busy. Please try again later.", HttpStatusCode.RequestTimeout, requestMethod, null, requestUri, null);

            return httpResponseMessage;
        }

        private void LogRetryException(Exception exception, int retryCount, Stopwatch stopwatch, string uri)
        {
            var et = new EventTelemetry("APIRetryPolicy");
            et.Properties.Add("Uri", uri);
            et.Properties.Add("Exception.Message", exception.Message);
            et.Properties.Add("Exception.StackTrace", exception.StackTrace);

            var innermostException = exception.GetBaseException();
            if (innermostException != null)
            {
                et.Properties.Add("InnermostException.Message", innermostException.Message);
                et.Properties.Add("InnermostException.StackTrace", innermostException.StackTrace);
            }
            et.Metrics.Add("RetryCount", retryCount);
            et.Metrics.Add("ElapsedTime", stopwatch.ElapsedMilliseconds);

            //tc.TrackEvent(et);
        }

        private void AddRecurrentHeaders(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (RecurrentRequestHeaders == null)
                return;

            foreach (var header in RecurrentRequestHeaders)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
        }

        #endregion
    }
}
