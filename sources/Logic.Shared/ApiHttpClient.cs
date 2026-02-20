using Data.Database;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Logic.Shared
{
    public class ApiHttpClient : IHttpClient
    {
        private readonly Logger<ApiHttpClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly ISecureStorageHandler _secureStorageHandler;
        private readonly Lazy<Task<bool>> _isApiAvailable;
        private static string BaseUrl => GetBaseUrl();
        public Task<bool> IsApiAvailableAsync => _isApiAvailable.Value;

        public ApiHttpClient(AppDbContext dbContext, ISecureStorageHandler secureStorageHandler)
        {
            _logger = new Logger<ApiHttpClient>(dbContext);
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                // Timeout = TimeSpan.FromSeconds(60)
            };
            _secureStorageHandler = secureStorageHandler;
            _isApiAvailable = new Lazy<Task<bool>>(CheckApiAvailabilityAsync);
        }

        public async Task<HttpResponseMessage> SendGetRequest(string url, Dictionary<string, object>? parameters = null)
        {
            var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey);

            if (token != null)
            {
                SetJwtToken(token);
            }

            var requestMessage = GetRequestMessage(url, HttpMethod.Get, parameters);

            var response = await _httpClient.SendAsync(requestMessage);

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

                    response = await SendGetRequest(url, parameters);
                }
                else
                {
                    throw new UnauthorizedAccessException("Token refresh failed.");
                }
            }

            return response;
        }

        public async Task<HttpResponseMessage> SendPostRequest<T>(string url, Dictionary<string, object>? parameters = null, T? model = default)
        {
            var token = await _secureStorageHandler.GetStringValue(StorageKeys.AccessTokenKey);

            if (token != null)
            {
                SetJwtToken(token);
            }

            var requestMessage = GetRequestMessage(url, HttpMethod.Post, parameters);

            requestMessage.Content = model != null
                        ? new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json")
                        : null;

            return await _httpClient.SendAsync(requestMessage);
        }

        public async Task<bool> CheckApiAvailabilityAsync()
        {
            try
            {
                var response = await SendGetRequest(
                    ApiConstants.HealthCheckEndpoint,
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
                var message = $"API availability check failed.";

                if (exception.InnerException != null)
                {
                    message += $"\nInner: {exception.InnerException.Message}";
                }

                await _logger.LogMessageAsync(message, LogMessageTypeEnum.Error, exception);

                return false;
            }
        }

        private HttpRequestMessage GetRequestMessage(string url, HttpMethod httpMethod, Dictionary<string, object>? parameters = null)
        {
            if (!IsUri(url))
            {
                url = $"{BaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
            }

            if (parameters != null && parameters.Count > 0)
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                foreach (string key in parameters.Keys)
                {
                    queryString[key] = parameters[key].ToString();
                }
                url += "?" + queryString.ToString();
            }

            return new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(url),
                Version = HttpVersion.Version11,
            };
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

            var response = await SendGetRequest("userauthentication/refreshtoken", null);

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

        private void SetJwtToken(string? jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
        }

        private bool IsUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out _);
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
