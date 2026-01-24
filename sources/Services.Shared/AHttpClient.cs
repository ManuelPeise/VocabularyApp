using Data.Database.Entities;
using System.Net.Http.Headers;

namespace Services.Shared
{
    public abstract class AHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed;
        
        protected HttpClient HttpClient => _httpClient;
        protected Func<LogMessageEntity, Task> LogMessageCallback;
        
        protected AHttpClient(string baseUrl, Func<LogMessageEntity, Task> logMessageCallback, string? jsonWebToken = null)
        {
            LogMessageCallback = logMessageCallback;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl, UriKind.Absolute),
                Timeout = TimeSpan.FromSeconds(30)
            };

            if (!string.IsNullOrEmpty(jsonWebToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jsonWebToken);
            }
        }
      
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}
