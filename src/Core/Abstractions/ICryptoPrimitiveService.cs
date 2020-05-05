using System.Threading.Tasks;
using Bit.Core.Enums;

namespace Bit.Core.Abstractions
{
    public interface ICryptoPrimitiveService
    {
        Task<byte[]> Pbkdf2(byte[] password, byte[] salt, CryptoHashAlgorithm algorithm, int iterations);
    }
}
