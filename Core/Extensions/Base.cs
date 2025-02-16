using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tycoon.Core.Extensions
{
    public static class Base
    {
        public static string ToFormattedString<T>(this List<T> list)
        {
            return $"[{string.Join(", ", list)}]";
        }

        public static string ToFormattedString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return $"{{{string.Join(", ", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"))}}}";
        }
    }
}
