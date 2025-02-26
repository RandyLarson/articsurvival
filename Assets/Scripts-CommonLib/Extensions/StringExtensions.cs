using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string lhs, string rhs)
        {
            if (lhs == null)
                return false;

            return string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase);
        }

        public static string OrEmpty(this string src)
        {
            return src ?? string.Empty;
        }
        public static string OrValue(this string src, string value)
        {
            return src ?? value;
        }

        public static bool HasContent(this string src)
        {
            return string.IsNullOrWhiteSpace(src) == false;
        }
        public static bool HasNoContent(this string src) => !src.HasContent();

        public static string SafeFormat(this string fmtString, params object[] args)
        {
            try
            {
                return string.Format(fmtString, args);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unable to format string: {fmtString.LogValue()}, msg: {ex.Message} ");
                return string.Empty;
            }
        }

        public static string Left(this string src, int len)
        {
            if (src == null) return null;
            if (src.Length >= len) return src[..len];
            return src;
        }

        public static string Left(this SerializableGuid src, int len) => ((Guid)src).Left(len);
        public static string Left(this Guid src, int len)
        {
            if (src == null) return string.Empty;
            return src.ToString().Left(len);
        }
    }
}
