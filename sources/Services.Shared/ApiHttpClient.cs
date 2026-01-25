using Data.Database;
using Services.Shared.Interfaces;
using Services.Shared.Models;
using Shared.Enums;
using System.Net;
using System.Text.Json;
using System.Web;
using Services.Shared.Constants;

    
namespace Services.Shared
{
    public class ApiHttpClient<TModel> : AHttpClient, IHttpClient<TModel> where TModel : class
    {
        private readonly Logger<ApiHttpClient<TModel>> _logger;
        private static string BaseUrl => GetBaseUrl();
        private readonly Lazy<Task<bool>> _isApiAvailable;
        private readonly ISecureStorageHandler _secureStorageHandler;
        public Task<bool> IsApiAvailableAsync => _isApiAvailable.Value;

        public ApiHttpClient(AppDbContext dbContext, ISecureStorageHandler secureStorageHandler) : base(BaseUrl)
        {
            _logger = new Logger<ApiHttpClient<TModel>>(dbContext);
            _isApiAvailable = new Lazy<Task<bool>>(CheckApiAvailabilityAsync);
            _secureStorageHandler = secureStorageHandler;

        }

        public async Task<ApiResponseBase<TModel>> GetAsync(
            string endpoint,
            Dictionary<string, object>? parameters  = null)
        {
            try
            {
                var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey) ?? null;

                if (token != null)
                {
                    SetJwtToken(token);
                }

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
                var message = $"Error while executing GET request [{endpoint}].";

                await _logger.LogMessageAsync(message, LogMessageTypeEnum.Error, exception);

                return new ApiResponseBase<TModel>
                {
                    Success = false,
                    ResponseData = null,
                    Error = message
                };
            }
        }

        public async Task<ApiResponseBase<TModel>> PostAsync<T>(string endpoint, T? model, Dictionary<string, object>? parameters = null)
        {
            try
            {
                var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey) ?? null;

                if (token != null)
                {
                    SetJwtToken(token);
                }

                var response = await SendAsync(endpoint, HttpMethod.Post, model, parameters);

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
                var message = $"Error while executing POST request [{endpoint}].";

                await _logger.LogMessageAsync(message, LogMessageTypeEnum.Error, exception);

                return new ApiResponseBase<TModel>
                {
                    Success = false,
                    ResponseData = null,
                    Error = message
                };
            }
        }

        private async Task<bool> CheckApiAvailabilityAsync()
        {
            try
            {
                var response = await SendAsync<object>(
                    ApiConstants.HealthCheckEndpoint, 
                    HttpMethod.Get, 
                    null, 
                    null);
                
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch(Exception exception)
            {
                var message = exception.Message;
                return false;
            }
        }

        private async Task<HttpResponseMessage> SendAsync<T>(string url, HttpMethod method, T? model, Dictionary<string, object>? parameters = null)
        {
            try
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
                    RequestUri = new Uri($"{BaseUrl}{url}"),
                    Content = model != null
                        ? new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json")
                        : null,
                    Version = HttpVersion.Version11,
                };

                var response = await HttpClient.SendAsync(requestMessage);

                return response;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending HTTP request to {url}: {ex.Message}", ex);
            }
        }

        private static string GetBaseUrl()
        {
#if ANDROID
            // Use 10.0.2.2 for Android emulator (maps to host machine)
            // Use actual IP for physical device
            return DeviceInfo.DeviceType == DeviceType.Virtual 
                ? ApiConstants.AndroidEmulatorUrl 
                : ApiConstants.LocalNetworkUrl;
#else
            return ApiConstants.LocalNetworkUrl;
#endif
        }
    }
}
    