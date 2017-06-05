using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalSlubny.Extensions
{
    public static class StringExtension
    {
        public static bool Contains(this string source, string text, StringComparison comparison)
        {
            return source.IndexOf(text, comparison) >= 0;
        }
    }
}