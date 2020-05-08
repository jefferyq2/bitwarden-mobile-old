using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.Extensions.Primitives;

namespace bw.web
{
    public static class Util
    {
        // Toggles the ref argument string between the values `password` and `text`
        public static void TogglePasswordInputType(ref string inputType) =>
            CycleValues(ref inputType, "password", "text");

        public static void CycleValues(ref string value, params string[] values)
        {
            int nextIndex = ((values?.IndexOf(value ?? "") ?? -1) + 1) % values.Length;
            value = values[nextIndex];
        }
    }
}
