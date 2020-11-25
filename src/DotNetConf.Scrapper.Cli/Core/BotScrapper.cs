using System;
using System.Collections.Generic;
using System.Linq;
using DotNetConf.Scrapper.Cli.Models;
using HtmlAgilityPack;

namespace DotNetConf.Scrapper.Cli.Core
{

    public class BotScrapper
    {
        private const string ATTR_SRC = "src";
        private readonly static Uri baseUrl = new Uri("https://mod-dotnet-bot.net");
        private readonly static Uri galleryUrl = new Uri(baseUrl, "gallery");

        public IEnumerable<Robot> ListRobots()
        {
            var doc = FetchHtmlDocument();
            return FindRobots(doc);

            static IEnumerable<Robot> FindRobots(HtmlDocument document)
            {
                var figs = document.DocumentNode.SelectNodes(@"//figure/img");

                foreach (var figure in figs)
                {
                    string attr = figure.Attributes.FirstOrDefault(el => el.Name == ATTR_SRC)?.Value
                            ?? throw new InvalidOperationException(nameof(FindRobots));
                    Uri fullFigureUrl = new Uri(baseUrl, attr); //$"{baseUrl}{attr}"
                    yield return new(fullFigureUrl.Segments.Last(), fullFigureUrl);
                }
            }
            static HtmlDocument FetchHtmlDocument()
            {
                var web = new HtmlWeb();
                var d = web.Load(galleryUrl);
                return d;
            }

        }

    }
}



