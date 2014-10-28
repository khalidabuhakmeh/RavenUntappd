using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;
using Scraper.Models;

namespace Scraper
{
    class Program
    {
        private static readonly IDocumentStore Store = new DocumentStore
        {
            ConnectionStringName = "RavenDB"
        }.Initialize(true);

        static void Main(string[] args)
        {
            var data = File.ReadAllText("checkin_export.json");
            var checkins = JsonConvert.DeserializeObject<List<Checkin>>(data);

            using (var bulk = Store.BulkInsert(options: new BulkInsertOptions { OverwriteExisting = true }))
            {
                for (int i = 0; i < checkins.Count; i++)
                {
                    var checkin = checkins[i];
                    checkin.Id = string.Format("checkin-{0}", i);
                    bulk.Store(checkin);
                }
            }

            var uniqueBreweryUrls = checkins.Select(x => x.BreweryUrl).Distinct();
            var breweries = new List<Brewery>();
            var configuration = new SessionConfiguration
            {
                Driver = typeof(SeleniumWebDriver),
                Browser = Browser.Chrome
            };

            using (var browser = new BrowserSession(configuration))
            {
                foreach (var unique in uniqueBreweryUrls)
                {
                    browser.Visit(unique);
                    browser.SaveScreenshot("brewery.png", ImageFormat.Png);
                    var location = browser.FindCss("p.brewery").Text;
                    var brewery = new Brewery
                    {
                        Id = unique.Replace("https://untappd.com/", ""),
                        Name = browser.FindCss(".basic .name h1").Text.Trim(),
                        RawLocation = location,
                        Style = browser.FindCss(".basic .name p.style").Text.Trim()
                    }.ProcessLocation(location);
                    breweries.Add(brewery);
                }
            }

            using (var bulk = Store.BulkInsert(options: new BulkInsertOptions { OverwriteExisting = true }))
            {
                foreach (var brewery in breweries)
                    bulk.Store(brewery);
            }

            Console.ReadLine();
        }
    }
}
