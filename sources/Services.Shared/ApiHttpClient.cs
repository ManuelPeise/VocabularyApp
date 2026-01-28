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

        public async Task<bool> CheckApiAvailabilityAsync()
        {
            try
            {
                var response = await SendAsync<string>(
                    ApiConstants.HealthCheckEndpoint,
                    HttpMethod.Get,
                    null,
                    null);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseString))
                {
                    return false;
                }

                return bool.Parse(responseString);
            }
            catch (Exception exception)
            {
                var message = $"API availability check failed.\nURL: {BaseUrl}{ApiConstants.HealthCheckEndpoint}\nError: {exception.Message}";
                if (exception.InnerException != null)
                {
                    message += $"\nInner: {exception.InnerException.Message}";
                }
                await _logger.LogMessageAsync(message, LogMessageTypeEnum.Error, exception);
                return false;
            }
        }

        public async Task<ApiResponseBase<TModel>> GetAsync(
            string endpoint,
            Dictionary<string, object>? parameters = null)
        {
            try
            {
                var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey);

                if (token != null)
                {
                    SetJwtToken(token);
                }

                var response = await SendAsync<TModel>(endpoint, HttpMethod.Get, null, parameters);

                // Handle token refresh if unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var refreshResult = await SendRefreshRequest();

                    if (refreshResult?.Result == true && 
                        !string.IsNullOrEmpty(refreshResult.AccessToken) && 
                        !string.IsNullOrEmpty(refreshResult.RefreshToken))
                    {
                        await _secureStorageHandler.SetValue(StorageKeys.AccessTokenKey, refreshResult.AccessToken);
                        await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, refreshResult.RefreshToken);

                        SetJwtToken(refreshResult.AccessToken);

                        response = await SendAsync<TModel>(endpoint, HttpMethod.Get, null, parameters);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Token refresh failed.");
                    }
                }

                response.EnsureSuccessStatusCode();

                TModel? responseModel = null;

                if (response.Content != null)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(json))
                    {
                        responseModel = JsonSerializer.Deserialize<TModel>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = false
                        });
                    }
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
                var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey);

                if (token != null)
                {
                    SetJwtToken(token);
                }

                var response = await SendAsync(endpoint, HttpMethod.Post, model, parameters);

                // Handle token refresh if unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var refreshResult = await SendRefreshRequest();

                    if (refreshResult?.Result == true &&
                        !string.IsNullOrEmpty(refreshResult.AccessToken) &&
                        !string.IsNullOrEmpty(refreshResult.RefreshToken))
                    {
                        await _secureStorageHandler.SetValue(StorageKeys.AccessTokenKey, refreshResult.AccessToken);
                        await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, refreshResult.RefreshToken);

                        SetJwtToken(refreshResult.AccessToken);

                        response = await SendAsync(endpoint, HttpMethod.Post, model, parameters);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Token refresh failed.");
                    }
                }

                response.EnsureSuccessStatusCode();

                TModel? responseModel = null;

                if (response.Content != null)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(json))
                    {
                        responseModel = JsonSerializer.Deserialize<TModel>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = false
                        });
                    }
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

        private async Task<AuthenticationResult?> SendRefreshRequest()
        {
            AuthenticationResult? result = null;

            var accessToken = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey) ?? null;
            var refreshToken = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey) ?? null;

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedAccessException();
            }

            var response = await SendAsync<AuthenticationResult>("userauthentication/refreshtoken", HttpMethod.Post, null);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync() ?? string.Empty;

            if (!string.IsNullOrEmpty(json))
            {
                result = JsonSerializer.Deserialize<AuthenticationResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false,

                }) ?? null;
            }
            return result;
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
            // Priority order for Android:
            // 1. Physical device via USB with ADB forwarding (most reliable for debugging)
            // 2. Emulator using 10.0.2.2
            // 3. Physical device on same WiFi network
            
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                // Android Emulator
                return ApiConstants.AndroidEmulatorUrl;
            }
            else
            {
                // Physical Device - Try USB forwarding first
                // To enable: Run in command prompt: adb forward tcp:5218 tcp:5218
                return ApiConstants.AndroidUsbUrl;
                
                // Alternative: Use WiFi network (if both devices on same network)
                // return ApiConstants.LocalNetworkUrl;
            }
#else
            return ApiConstants.LocalhostUrl;
#endif
        }
    }
}
