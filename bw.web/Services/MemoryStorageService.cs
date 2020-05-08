using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Blazored.LocalStorage;

namespace bw.web.Services
{
    public class MemoryStorageService : IStorageService
    {
        private Dictionary<string, object> _store = new Dictionary<string, object>();

        public MemoryStorageService()
        { }

        public Task SaveAsync<T>(string key, T obj)
        {
            if (obj == null)
                _store.Remove(key);
            else
                _store[key] = obj;

            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (_store.TryGetValue(key, out var value) && value is T tValue)
                return Task.FromResult(tValue);
            return Task.FromResult(default(T));
        }

        public Task RemoveAsync(string key)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }
    }
}

