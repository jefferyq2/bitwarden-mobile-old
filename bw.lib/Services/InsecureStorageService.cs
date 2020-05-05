using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;

namespace bw.lib.Services
{
    public class InsecureStorageService : IStorageService
    {
        private Dictionary<string, string> _map = new Dictionary<string, string>();

        public InsecureStorageService()
        {
            Console.Error.WriteLine("WARNING:  using EPHEMERAL Secure Storage Service");
        }

        public Task SaveAsync<T>(string key, T obj)
        {
            if (typeof(T) == typeof(string))
                _map[key] = obj as string;
            else
                _map[key] = JsonSerializer.Serialize(obj);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            Console.Error.WriteLine("Getting value for key {0}", key);
            if (!_map.TryGetValue(key, out var str))
                return Task.FromResult(default(T));

            if (typeof(T) == typeof(string))
                return Task.FromResult((T)(object)str);
            else
                return Task.FromResult(JsonSerializer.Deserialize<T>(str));
        }

        public Task RemoveAsync(string key)
        {
            _map.Remove(key);
            return Task.CompletedTask;
        }
    }
}
