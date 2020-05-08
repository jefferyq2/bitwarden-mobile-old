using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Blazored.SessionStorage;

namespace bw.web.Services
{
    public class BrowserSecureStorageService : IStorageService
    {
        private const string KeyPrefix = "SECURE:";

        private ISessionStorageService _session;

        public BrowserSecureStorageService(ISessionStorageService session)
        {
            _session = session;
        }

        public Task SaveAsync<T>(string key, T obj)
        {
            string str;
            if (typeof(T) == typeof(string))
                str = obj as string;
            else
                str = JsonSerializer.Serialize(obj);

            return _session.SetItemAsync(KeyPrefix + key, str);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var str = await _session.GetItemAsync<string>(KeyPrefix + key);
            if (str == null)
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)str;
            else
                return JsonSerializer.Deserialize<T>(str);
        }

        public Task RemoveAsync(string key)
        {
            return _session.RemoveItemAsync(KeyPrefix + key);
        }
    }
}
