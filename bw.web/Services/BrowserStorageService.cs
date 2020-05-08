using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace bw.web.Services
{
    public class BrowserStorageService : IStorageService
    {
        // We define a list of keys that should be stored in the browser's
        // "Local" storage, anything else automatically goes to "Session"
        private static IReadOnlyList<string> LocalStorageKeys = new List<string>
        {
            "appId", "anonymousAppId",
            "rememberedEmail", "passwordGenerationOptions",
            ServiceConstants.DisableFaviconKey,
            "rememberEmail", "enableGravatars",
            ServiceConstants.LocaleKey,
            ServiceConstants.AutoConfirmFingerprints, ServiceConstants.VaultTimeoutKey,
            ServiceConstants.VaultTimeoutActionKey,
        };

        private ILocalStorageService _local;
        private ISessionStorageService _session;

        public BrowserStorageService(ILocalStorageService local, ISessionStorageService session)
        {
            _local = local;
            _session = session;
        }

        public Task SaveAsync<T>(string key, T obj)
        {
            string str;
            if (typeof(T) == typeof(string))
                str = obj as string;
            else
                str = JsonSerializer.Serialize(obj);

            if (LocalStorageKeys.Contains(key))
                return _local.SetItemAsync(key, str);
            return _session.SetItemAsync(key, str);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            string str;

            if (LocalStorageKeys.Contains(key))
                str = await _local.GetItemAsync<string>(key);
            else
                str = await _session.GetItemAsync<string>(key);
            
            if (str == null)
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)str;
            else
                return JsonSerializer.Deserialize<T>(str);
        }

        public Task RemoveAsync(string key)
        {
            if (LocalStorageKeys.Contains(key))
                return _local.RemoveItemAsync(key);
            return _session.RemoveItemAsync(key);
        }
    }
}
