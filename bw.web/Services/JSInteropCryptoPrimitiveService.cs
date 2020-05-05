using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Enums;
using Microsoft.JSInterop;

namespace bw.web.Services
{
    public class JSInteropCryptoPrimitiveService : ICryptoPrimitiveService
    {
        private IJSRuntime _js;

        public JSInteropCryptoPrimitiveService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<byte[]> Pbkdf2(byte[] password, byte[] salt, CryptoHashAlgorithm algorithm, int iterations)
        {
            var b64 = await _js.InvokeAsync<string>("bw_web.pbkdf2",
                Convert.ToBase64String(password),
                Convert.ToBase64String(salt),
                iterations);
            return Convert.FromBase64String(b64);
        }
    }
}
