using System;

namespace Assets.Scripts.Extensions
{
    public static class LogicExtensions
    {
        public static void iff<T>(this T src, Action<T> fn)
        {
            if (src != null)
                fn(src);
        }

        public static T ifnull<T>(this T src, Func<T,T> fn)
        {
            if (src == null)
                return fn(src);

            return src;
        }
    }
}
