using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;

namespace GitHubDependentsScraper
{
    class Program
    {
        private const string SELECTOR_DEPENDENCY = ".Box-row.d-flex.flex-items-center";
        private const string SELECTOR_BUTTONS = "a.btn.btn-outline.BtnGroup-item";
        private const string HELP = @"GitHub Dependents Scraper: a CLI to get a sorted list of dependencies of a project on GitHub.

USAGE
  $ gh-dep-scraper <URL>
";
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine(HELP);
                return;
            }

            string url = args[0];
            Dictionary<int, List<Dependency>> dependencies = new Dictionary<int, List<Dependency>>();
            using (IWebDriver driver = new ChromeDriver())
            {
                ProcessPage(OnPageParsed, 1, dependencies, driver, url);
            }
        }

        private static void OnPageParsed(string url, int pageNumber, Dictionary<int, List<Dependency>> dependencies)
        {
            Console.WriteLine($"Page {pageNumber} processed. Caching results...");
            Cache(url, pageNumber, dependencies);
        }

        private static void Cache(string url, int pageNumber, Dictionary<int, List<Dependency>> dependencies)
        {
            Dictionary<string, object> cacheFile = new Dictionary<string, object>
            {
                { "page", pageNumber },
                { "data", dependencies }
            };
            File.WriteAllText(UrlUtil.GetUrlPathSafe(url), JsonConvert.SerializeObject(cacheFile, Formatting.Indented));
        }

        private static void ProcessPage(Action<string, int, Dictionary<int, List<Dependency>>> onPageParsed, int pageNumber, Dictionary<int, List<Dependency>> dependencies, IWebDriver driver, string url)
        {
            driver.Url = url;
            ReadOnlyCollection<IWebElement> rows = driver.FindElements(By.CssSelector(SELECTOR_DEPENDENCY));
            foreach (IWebElement row in rows)
            {
                Dependency dependency = ParseDependency(row);
                if (dependency.Stars > 0)
                {
                    if (dependencies.ContainsKey(dependency.Stars))
                    {

                    }
                    else
                    {
                        dependencies.Add(dependency.Stars, new Dependency[] { dependency }.ToList());
                    }
                }
            }

            IWebElement nextButton = driver.FindElements(By.CssSelector(SELECTOR_BUTTONS)).ToList().Last();
            if (nextButton.Text == "Next")
            {
                onPageParsed(url, pageNumber, dependencies);
                ProcessPage(onPageParsed, pageNumber + 1, dependencies, driver, nextButton.GetAttribute("href"));
            }
            else
            {
                PrintResults(dependencies);
            }
        }

        private static void PrintResults(Dictionary<int, List<Dependency>> dependencies)
        {
            List<int> sortedKeys = dependencies.Keys.ToList().OrderBy(x => x).ToList();
            foreach (int key in sortedKeys)
            {
                List<Dependency> sortedDependencies = dependencies[key].OrderBy(x => x.Name).ToList();
                foreach (Dependency dependency in sortedDependencies)
                {
                    PrintResult(dependency);
                }
            }
        }

        private static void PrintResult(Dependency dependency)
        {
            Console.WriteLine($"Name: {dependency.Name} - Stars: {dependency.Stars}");
            Console.WriteLine($"\tAuthor: {dependency.Author}\t|\tURL: {dependency.URL}\t|\tForks: {dependency.Forks}");
        }

        private static Dependency ParseDependency(IWebElement row)
        {
            Dependency result = new Dependency();
            IWebElement userDataElement = row.FindElement(By.CssSelector(".f5.text-gray-light"));
            ReadOnlyCollection<IWebElement> userDataAnchors = userDataElement.FindElements(By.CssSelector("a"));
            IWebElement userLink = userDataAnchors.First();
            result.Author = userLink.Text;
            IWebElement packageLink = userDataAnchors.Last();
            result.Name = packageLink.Text;
            result.URL = packageLink.GetAttribute("href");
            ReadOnlyCollection<IWebElement> starAndForkElements = row.FindElements(By.CssSelector(".text-gray-light.text-bold.pl-3"));
            IWebElement starElement = starAndForkElements.First();
            IWebElement forkElement = starAndForkElements.Last();
            result.Stars = int.Parse(starElement.Text);
            result.Forks = int.Parse(forkElement.Text);
            return result;
        }
    }
}
