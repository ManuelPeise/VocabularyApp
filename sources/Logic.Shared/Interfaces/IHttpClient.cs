namespace Logic.Shared.Interfaces
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendGetRequest(string url, Dictionary<string, object>? parameters = null);
        Task<HttpResponseMessage> SendPostRequest<T>(string url, Dictionary<string, object>? parameters = null, T? model = default);
        Task<bool> IsApiAvailableAsync { get; }
    }
}
