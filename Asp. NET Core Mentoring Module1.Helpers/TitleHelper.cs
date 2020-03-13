using System.Text.RegularExpressions;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public static class TitleHelper
    {
        private static readonly string _pattern = @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))";

        public static string FormatTitle(string title)
        {
            return Regex.Replace(title, _pattern, " $0");
        }
    }
}
