namespace Core.App.Services.Interfaces
{
    public interface ISecureStorageHandler
    {
        Task SetAsync(string key, string value);
        Task<string?> GetAsync(string key);
        bool Remove(string key);
        void RemoveAll();
    }
}
