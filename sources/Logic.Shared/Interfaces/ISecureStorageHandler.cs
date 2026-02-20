namespace Logic.Shared.Interfaces
{
    public interface ISecureStorageHandler
    {
        Task<T?> GetValue<T>(string key);
        Task<string?> GetStringValue(string key);
        Task SetValue<T>(string key, T model);
        Task SetValue(string key, string value);
        void RemoveValue(string key);
    }
}
