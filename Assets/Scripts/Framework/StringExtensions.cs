using System;
using System.Linq;
namespace Assets.Scripts
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static int CompareToNullSafe(this string one, string two)
        {
            if (one == null ^ two == null)
            {
                return (one == null) ? -1 : 1;
            }

            if (one == null && two == null)
            {
                return 0;
            }

            return one.CompareTo(two);
        }

        public static string FormatStr(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}