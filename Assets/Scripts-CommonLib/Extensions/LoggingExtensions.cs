namespace Assets.Scripts.Extensions
{
    public static class LoggingExtensions
    {
        public static string LogValue(this string src)
        {
            return src == null ? "(null)" : $"`{src}`";
        }
    }
}
