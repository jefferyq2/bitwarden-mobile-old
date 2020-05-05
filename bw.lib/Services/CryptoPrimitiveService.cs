using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Enums;

namespace bw.lib.Services
{
    public class CryptoPrimitiveService : ICryptoPrimitiveService
    {
        public Task<byte[]> Pbkdf2(byte[] password, byte[] salt, CryptoHashAlgorithm algorithm, int iterations)
        {
            int keySize;
            HashAlgorithmName algor;
            switch (algorithm)
            {
                case CryptoHashAlgorithm.Sha256:
                    keySize = 256;
                    algor = HashAlgorithmName.SHA256;
                    break;
                case CryptoHashAlgorithm.Sha512:
                    keySize = 512;
                    algor = HashAlgorithmName.SHA512;
                    break;
                default:
                    throw new ArgumentException($"Unsupported PBKDF2 algorithm ({algorithm})",
                        nameof(algorithm));
            }

            var keyByteSize = keySize / 8;
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations, algor))
            {
                return Task.FromResult(kdf.GetBytes(keyByteSize));
            }
        }
    }
}
