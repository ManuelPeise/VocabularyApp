using Services.Shared.Models;

namespace Services.Shared.Interfaces
{
    public interface IHttpClient<TModel> where TModel : class
    {
        Task<bool> IsApiAvailableAsync { get; }
        Task<ApiResponseBase<TModel>> GetAsync(
            string endpoint,
            Dictionary<string, object>? parameters = null);
        Task<ApiResponseBase<TModel>> PostAsync<T>(
            string endpoint,
            T? model,
            Dictionary<string, object>? parameters = null);
    }
}
