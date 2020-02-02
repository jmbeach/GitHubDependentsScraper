using GitHubDependentsScraper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GitHubDependentsScraperTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetUrlPathSafeTest()
        {
            string expected = "npmvalidate-npm-package-namenetworkdependents";
            string url = "https://github.com/npm/validate-npm-package-name/network/dependents";
            string url2 = "https://github.com/npm/validate-npm-package-name/network/dependents?dependents_after=MTE1Mzg0MTE1MTk";
            Assert.AreEqual(expected, UrlUtil.GetUrlPathSafe(url));
            Assert.AreEqual(expected, UrlUtil.GetUrlPathSafe(url2));
        }

        [TestMethod]
        public void GetDependentsAfterTest()
        {
            string expected = "MTE1Mzg0MTE1MTk";
            string url = "https://github.com/npm/validate-npm-package-name/network/dependents";
            string url2 = "https://github.com/npm/validate-npm-package-name/network/dependents?dependents_after=MTE1Mzg0MTE1MTk";
            Assert.AreEqual(null, UrlUtil.GetDependentsAfter(url));
            Assert.AreEqual(expected, UrlUtil.GetDependentsAfter(url2));
        }
    }
}
