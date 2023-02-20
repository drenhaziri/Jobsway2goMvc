namespace Jobsway2goMvc.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirst(this string value)
        {
            value = value.ToLower();
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}
