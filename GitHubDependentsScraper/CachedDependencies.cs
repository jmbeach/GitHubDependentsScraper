using System.Collections.Generic;

namespace GitHubDependentsScraper
{
    public class CachedDependencies
    {
        public int Page { get; set; }
        public string After { get; set; }
        public Dictionary<int, List<Dependency>> Data { get; set; }
    }
}
