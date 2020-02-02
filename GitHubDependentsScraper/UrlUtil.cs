using System.Text.RegularExpressions;

namespace GitHubDependentsScraper
{
    public static class UrlUtil
    {
        private static readonly Regex _urlReplacer = new Regex(@"[:/]|https://github.com|(\?.*)", RegexOptions.Compiled);
        private static readonly Regex _dependentsAfterMatcher = new Regex(@"dependents_after=(.*)", RegexOptions.Compiled);

        public static string GetUrlPathSafe(string url)
        {
            return _urlReplacer.Replace(url, string.Empty);
        }

        public static string GetDependentsAfter(string url)
        {
            Match match = _dependentsAfterMatcher.Match(url);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}
