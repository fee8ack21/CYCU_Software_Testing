namespace App.Common.Extensions
{
    public static class StringExtension
    {
        public static string ToCamel(this string str) =>
            Char.ToLowerInvariant(str[0]) + str.Substring(1);

        public static string ToPascal(this string str) =>
            Char.ToUpperInvariant(str[0]) + str.Substring(1);
    }
}
