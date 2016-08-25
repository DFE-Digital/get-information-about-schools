using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using Web.Services.Api.Models;

namespace Web.Services.Api
{
    public class HttpService : IHttpService
    {
        private readonly IAppSettings _appSettings;

        public HttpService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public dynamic ExecuteGet(string resource)
        {
            return ExecuteRequest(resource);
        }

        public DownloadResponse DownloadFile(string url)
        {
            var encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_appSettings.ApiUserName + ":" + _appSettings.ApiPassword));

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_appSettings.ApiBaseUrl + url);
            httpRequest.Headers.Add("Accept-Encoding", "gzip");
            httpRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            httpRequest.Headers.Add("Authorization", "Basic " + encoded);

            httpRequest.Method = "GET";

            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    using (Stream responseStream = httpResponse.GetResponseStream())
                    {
                        byte[] buffer = new byte[16 * 1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            return new DownloadResponse
                            {
                                ResponseContents = ms.ToArray(),
                                FileName = httpResponse.Headers["Content-Disposition"].Substring(httpResponse.Headers["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", ""),
                                ContentType = httpResponse.ContentType,
                                NotFound = false
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new DownloadResponse
                {
                    NotFound = true
                };
            }

        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public dynamic ExecutePost(string resource, object body)
        {
            return ExecuteRequest(resource, Method.POST, body);
        }

        private dynamic ExecuteRequest(string resource, Method method = Method.GET, object body = null)
        {
            return ExecuteWithRetry(() =>
            {
                var client = new RestClient(_appSettings.ApiBaseUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(_appSettings.ApiUserName, _appSettings.ApiPassword)
                };

                var request = new RestRequest(resource)
                {
                    Method = method
                };

                if (body != null)
                {
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(body);
                }

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    try
                    {
                        return JObject.Parse(response.Content);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            return JArray.Parse(response.Content);
                        }
                        catch (JsonReaderException)
                        {
                            if (response.Content == "null")
                                return null;

                            throw;
                        }
                    }
                }

                return new ApiErrorResponse(response.StatusCode, response.Content ?? "No items were returned");
            });
        }

        public T ExecuteGet<T>(string resource)
        {
            return ExecuteWithRetry(() =>
            {
                var client = new RestClient(_appSettings.ApiBaseUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(_appSettings.ApiUserName, _appSettings.ApiPassword)
                };

                var request = new RestRequest(resource);
                request.AddHeader("Accept-Encoding", "gzip,deflate");

                IRestResponse response = client.Execute(request);

                try
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception)
                {
                    return new ApiErrorResponse(response.StatusCode, "Couldn't parse response");
                }
            });
        }

        private static dynamic ExecuteWithRetry(Func<dynamic> action)
        {
            const int maxRetries = 4;

            bool done = false;
            int attempts = 0;

            while (!done)
            {
                attempts++;
                try
                {
                    var response = action();
                    var errorResponse = response as ApiErrorResponse;
                    if (errorResponse != null)
                    {
                        throw new WebException(errorResponse.Message, WebExceptionStatus.ReceiveFailure);
                    }
                    done = true;
                    return response;
                }
                catch (WebException ex)
                {
                    if (!IsRetryable(ex))
                    {
                        throw;
                    }

                    if (attempts >= maxRetries)
                    {
                        throw;
                    }

                    Thread.Sleep(SleepTime(attempts));
                }
            }

            throw new WebException("Connection Failure", WebExceptionStatus.ConnectFailure);
        }

        private static int SleepTime(int retryCount)
        {
            switch (retryCount)
            {
                case 0: return 0;
                case 1: return 1000;
                case 2: return 2000;
                case 3: return 2000;
                default: return 5000;
            }
        }

        private static bool IsRetryable(WebException ex)
        {
            return
                ex.Status == WebExceptionStatus.ReceiveFailure ||
                ex.Status == WebExceptionStatus.ConnectFailure ||
                ex.Status == WebExceptionStatus.KeepAliveFailure;
        }
    }
}