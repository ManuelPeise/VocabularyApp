using Logic.Shared.Interfaces;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace Logic.Shared
{
    public class SecureStorageHandler : ISecureStorageHandler
    {
        public async Task<T?> GetValue<T>(string key)
        {
            var json = await SecureStorage.GetAsync(key);

            if (json == null || json.Length == 0)
            {
                return default(T?);
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<string?> GetStringValue(string key)
        {
            var json = await SecureStorage.GetAsync(key);

            return json ?? null;
        }

        public async Task SetValue<T>(string key, T model)
        {
            var json = JsonSerializer.Serialize(model) ?? string.Empty;

            await SecureStorage.SetAsync(key, json);
        }

        public async Task SetValue(string key, string value)
        {
            await SecureStorage.SetAsync(key, value);
        }

        public void RemoveValue(string key)
        {
            SecureStorage.Remove(key);
        }


    }
}
