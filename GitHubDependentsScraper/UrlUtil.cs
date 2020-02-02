using System.Linq;
using System.Text.RegularExpressions;

namespace GitHubDependentsScraper
{
    public static class UrlUtil
    {
        private static readonly Regex _urlReplacer = new Regex(@"[:/]|https://github.com|(\?.*)", RegexOptions.Compiled);
        public static string GetUrlPathSafe(string url)
        {
            return _urlReplacer.Replace(url, string.Empty);
        }
    }
}
