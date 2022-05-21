using BlackdotTask.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//using BlackdotTask.Models.SearchInput;

namespace BlackdotTask.Controllers
{
    public class HomeController : Controller
    {
        //static SearchInput searchInput = new SearchInput();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost, Route("GetResults")]
        public IActionResult GetResults(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                // Instantiate an HtmlWeb object to navigate
                HtmlWeb web = new HtmlWeb();

                // Load google search engine with the parameter
                string google = "https://www.google.com/search?q=" + param;
                HtmlDocument googleSite = web.Load(google);

                // Create a list of strings and extract all href tags from first results from google
                List<string> hrefTagsGoogle = new List<string>();
                hrefTagsGoogle = ExtractAllAHrefTags(googleSite);

                // Load search encrypt search engine with the parameter
                string searchencrypt = "https://www.searchencrypt.com/search?st=web&q=" + param;
                HtmlDocument searchencryptSite = web.Load(searchencrypt);

                List<string> hrefTagsSearchencrypt = new List<string>();
                hrefTagsSearchencrypt = ExtractAllAHrefTags(searchencryptSite);

                // Concat both Lists and return only the distincts value
                List<string> hrefTags = hrefTagsGoogle.Concat(hrefTagsSearchencrypt).Distinct().ToList();
                ViewData["MyData"] = hrefTags;
                ViewData["Message"] = param;

                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        private List<string> ExtractAllAHrefTags(HtmlDocument document)
        {
            List<string> hrefTags = new List<string>();

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                if (att.Value.StartsWith("https") && !att.Value.Contains("search"))
                {
                    hrefTags.Add(att.Value);
                }                
            }

            return hrefTags;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
