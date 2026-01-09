using Core.App.Services.Interfaces;

namespace Core.App.Services
{
    public class SecureStorageHandler: ISecureStorageHandler
    {
        public async Task SetAsync(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);
            await SecureStorage.Default.SetAsync(key, value);
        }

        public async Task<string?> GetAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return await SecureStorage.Default.GetAsync(key);
        }

        public bool Remove(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return SecureStorage.Default.Remove(key);
        }

        public void RemoveAll()
        {
            SecureStorage.Default.RemoveAll();
        }
    }
}
