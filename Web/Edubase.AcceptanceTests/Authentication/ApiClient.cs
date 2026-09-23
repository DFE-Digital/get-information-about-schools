using System.Collections;
using System.Net;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Edubase.AcceptanceTests.Authentication;

public sealed class ApiClient
{
    public HttpClient Client { get; }
    private readonly IMessageSink _sink;

    public ApiClient(HttpClient client, IMessageSink sink)
    {
        Client = client;
        _sink = sink;
    }

    /// <summary>
    /// HTTP GET with response type TResponse
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string path) where TResponse : class, new()
    {
        var absolutePath = Client.BaseAddress.ToString() + path;
        var response = await Client.GetAsync(absolutePath);

        string rawContent = await response.Content.ReadAsStringAsync();

        // Check if response is successful and content is JSON
        // If the HTTP response is unsuccessful or the content type is not JSON,
        // log diagnostic details including the full request path, status code, and raw response body.
        // Then return an ApiResponse with a null Body and the raw JSON string captured for debugging.

        if (!IsValidJsonResponse(response))
        {
            LogDiagnostic(nameof(GetAsync), absolutePath, response.StatusCode, rawContent);
            return new ApiResponse<TResponse>
            {
                StatusCode = response.StatusCode,
                Body = null,
                RawJson = rawContent
            };
        }

        // Optional: Try to parse as JObject for logging
        JToken deserializedObject = null;
        try
        {
            deserializedObject = JsonConvert.DeserializeObject<JToken>(rawContent);
        }
        catch (JsonReaderException ex)
        {
            _sink.OnMessage(new DiagnosticMessage($"[Deserialization Error] Could not parse response as JObject: {ex.Message}"));
        }

        // Log the output
        _sink.OnMessage(new DiagnosticMessage(
            $"\n {nameof(GetAsync)} \n path: {absolutePath} \n status code: {response.StatusCode} \n response body: {(deserializedObject != null ? deserializedObject.ToString() : rawContent)}"));

        var bodyResponse = SafeDeserialise<TResponse>(rawContent);

        return new ApiResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Body = bodyResponse,
            RawJson = rawContent
        };
    }

    /// <summary>
    /// HTTP POST response type TResponse
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="path"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string path, TRequest body)
        where TRequest : class
        where TResponse : class, new()
    {
        var serializedBody = JsonConvert.SerializeObject(body);
        var deserializedObject = JsonConvert.DeserializeObject<JObject>(serializedBody);
        _sink.OnMessage(new DiagnosticMessage($"\n{nameof(PostAsync)}\nrequest body: {deserializedObject}"));

        var absolutePath = Client.BaseAddress.ToString() + path;
        var response = await Client.PostAsync(
            absolutePath,
            new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json)
        );

        var rawContent = await response.Content.ReadAsStringAsync();

        if (!IsValidJsonResponse(response))
        {
            LogDiagnostic(nameof(PostAsync), absolutePath, response.StatusCode, rawContent);
            return new ApiResponse<TResponse>
            {
                StatusCode = response.StatusCode,
                Body = null,
                RawJson = rawContent
            };
        }

        LogDiagnostic(nameof(PostAsync), absolutePath, response.StatusCode, rawContent);

        var bodyResponse = SafeDeserialise<TResponse>(rawContent);

        return new ApiResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Body = bodyResponse,
            RawJson = rawContent
        };
    }

    /// <summary>
    /// HTTP PUT response type TResponse
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="path"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string path, TRequest body)
    where TRequest : class
    where TResponse : class, new()
    {
        var serializedBody = JsonConvert.SerializeObject(body);
        var deserializedObject = JsonConvert.DeserializeObject<JObject>(serializedBody);
        _sink.OnMessage(new DiagnosticMessage($"\n{nameof(PutAsync)}\nrequest body: {deserializedObject}"));

        var absolutePath = Client.BaseAddress.ToString() + path;
        var response = await Client.PutAsync(
            absolutePath,
            new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json)
        );

        var rawContent = await response.Content.ReadAsStringAsync();

        if (!IsValidJsonResponse(response))
        {
            LogDiagnostic(nameof(PostAsync), absolutePath, response.StatusCode, rawContent);
            return new ApiResponse<TResponse>
            {
                StatusCode = response.StatusCode,
                Body = null,
                RawJson = rawContent
            };
        }

        LogDiagnostic(nameof(PutAsync), absolutePath, response.StatusCode, rawContent);

        var bodyResponse = SafeDeserialise<TResponse>(rawContent);

        return new ApiResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Body = bodyResponse,
            RawJson = rawContent
        };
    }

    /// <summary>
    /// HTTP Patch no response type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="path"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<ApiResponse<HttpResponseMessage>> PatchAsync<TRequest>(string path, TRequest body)
    where TRequest : class
    {
        var serializedBody = JsonConvert.SerializeObject(body);
        var deserializedObject = JsonConvert.DeserializeObject<JObject>(serializedBody);
        _sink.OnMessage(new DiagnosticMessage($"\n{nameof(PatchAsync)}\nrequest body:\n{deserializedObject}"));

        var absolutePath = Client.BaseAddress.ToString() + path;
        var response = await Client.PatchAsync(
            absolutePath,
            new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json)
        );

        var rawContent = await response.Content.ReadAsStringAsync();
        LogDiagnostic(nameof(PatchAsync), absolutePath, response.StatusCode, rawContent);

        return new ApiResponse<HttpResponseMessage>
        {
            StatusCode = response.StatusCode,
            Body = response,
            RawJson = rawContent
        };
    }

    /// <summary>
    /// HTTP Delete with response type TResponse
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> DeleteAsync<TResponse>(string path) where TResponse : class
    {
        var absolutePath = $"{Client.BaseAddress}{path}";
        var response = await Client.DeleteAsync(absolutePath);

        var rawContent = await response.Content.ReadAsStringAsync();

        LogDiagnostic(nameof(DeleteAsync), absolutePath, response.StatusCode, rawContent);

        response.EnsureSuccessStatusCode();
        return response;
    }


    private bool IsValidJsonResponse(HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.ContentType?.MediaType;
        return contentType == MediaTypeNames.Application.Json;
    }

    public void LogDiagnostic(string methodName, string path, HttpStatusCode statusCode, string body)
    {
        var message = new DiagnosticMessage(
            $"\n{methodName}\npath: {path}\nstatus code: {statusCode}\nresponse body: {body}"
        );

        _sink.OnMessage(message); // This line will throw if _sink is null
    }

    /// <summary>
    /// Uses JToken.Parse() to inspect the JSON structure before attempting deserialisation.
    /// If the response is a JSON array and T is not a collection type, it skips deserialisation and logs a diagnostic message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public T? SafeDeserialise<T>(string json)
    {
        try
        {
            var token = JToken.Parse(json);

            // If the token is an array but T is not a collection, skip deserialisation
            if (token.Type == JTokenType.Array && !typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                _sink.OnMessage(new DiagnosticMessage(
                    $"[Deserialisation Skipped] JSON is array but target type is {typeof(T).Name}"));
                return default;
            }

            return token.ToObject<T>();
        }
        catch (JsonException ex)
        {
            _sink.OnMessage(new DiagnosticMessage($"[Deserialisation Error] {ex.Message}"));
            return default;
        }
    }

}

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? Body { get; set; }
    public T? RawContent { get; set; }
    public string? RawJson { get; set; }
}
