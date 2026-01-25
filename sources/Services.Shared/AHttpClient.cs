using Data.Database.Entities;
using System.Net.Http.Headers;

namespace Services.Shared
{
    public abstract class AHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed;
        
        protected HttpClient HttpClient => _httpClient;
      
        protected AHttpClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl, UriKind.Absolute),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }
        protected void SetJwtToken(string? jwt)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
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
