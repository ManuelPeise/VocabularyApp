using Data.Database.Entities;
using Shared.Enums;
using Shared.Interfaces;
using Shared.Models;
using System.Net;
using System.Text.Json;
using System.Web;

    
namespace Services.Shared
{
    public class ApiHttpClient<TModel> : AHttpClient, IHttpClient<TModel> where TModel : class
    {

        private const string BaseUrl = "http://192.168.178.46:5000/api/";
        private readonly Lazy<Task<bool>> _isApiAvailable;
        public Task<bool> IsApiAvailableAsync => _isApiAvailable.Value;

        public ApiHttpClient(Func<LogMessageEntity, Task> logMessageCallback, string? jsonWebToken = null) : base(BaseUrl, logMessageCallback, jsonWebToken)
        {
            _isApiAvailable = new Lazy<Task<bool>>(CheckApiAvailabilityAsync);
        }

        public async Task<ApiResponseBase<TModel>> GetAsync(
            string endpoint,
            Dictionary<string, object>? parameters  = null)
        {
            try
            {
                var response = await SendAsync<object>(endpoint, HttpMethod.Get, parameters, null);

                response.EnsureSuccessStatusCode();

                TModel? responseModel = null;

                if (response.Content != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync() ?? string.Empty;

                    responseModel = JsonSerializer.Deserialize<TModel>(jsonString) ?? null;
                }

                return new ApiResponseBase<TModel>
                {
                    Success = true,
                    ResponseData = responseModel
                };
            }
            catch (Exception exception)
            {
                var entity = new LogMessageEntity
                {
                    Message = $"Error while executing GET request [{endpoint}].",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = "ApiHttpClient",
                    LogLevel = LogLevelEnum.Error,
                };

                await LogMessageCallback(entity);

                return new ApiResponseBase<TModel>
                {
                    Success = false,
                    ResponseData = null,
                    Error = entity.Message
                };
            }
        }

        public async Task<ApiResponseBase<TModel>> PostAsync<T>(string endpoint, T? model, Dictionary<string, object>? parameters = null)
        {
            try
            {
                var response = await SendAsync(endpoint, HttpMethod.Post, parameters, model);

                response.EnsureSuccessStatusCode();

                TModel? responseModel = null;

                if (response.Content != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync() ?? string.Empty;
                    responseModel = JsonSerializer.Deserialize<TModel>(jsonString) ?? null;
                }

                return new ApiResponseBase<TModel>
                {
                    Success = true,
                    ResponseData = responseModel
                };
            }
            catch (Exception exception)
            {
                var entity = new LogMessageEntity
                {
                    Message = $"Error while executing POST request [{endpoint}].",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = "ApiHttpClient",
                    LogLevel = LogLevelEnum.Error,
                };

                await LogMessageCallback(entity);

                return new ApiResponseBase<TModel>
                {
                    Success = false,
                    ResponseData = null,
                    Error = entity.Message
                };
            }
        }

        private async Task<bool> CheckApiAvailabilityAsync()
        {
            try
            {
                var response = await SendAsync<object>("health", HttpMethod.Get, null, null);
                
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private async Task<HttpResponseMessage> SendAsync<T>(string url, HttpMethod method, Dictionary<string, object>? parameters = null, T? model)
        {
            if (parameters != null && parameters.Count > 0)
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                foreach (string key in parameters.Keys)
                {
                    queryString[key] = parameters[key].ToString();
                }

                url += "?" + queryString.ToString();
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url, UriKind.Relative),
                Content = model != null
                    ? new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json")
                    : null,
                Version = HttpVersion.Version11,
            };

            return await HttpClient.SendAsync(requestMessage);
        }


       
    }
}
