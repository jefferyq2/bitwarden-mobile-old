using System.Globalization;

namespace bw.lib.Abstractions
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();
    }
}
