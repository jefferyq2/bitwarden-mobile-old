using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bw.lib.Abstractions;

namespace bw.lib.Services
{
    public class LocalizeService : ILocalizeService
    {
        private static readonly CultureInfo CurrentCultureInfo =
            CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;

        public CultureInfo GetCurrentCultureInfo() => CurrentCultureInfo;
            
    }
}
