#r "nuget: HtmlAgilityPack, 1.11.28"

#nullable enable

using HtmlAgilityPack;

var baseUrl = new Uri("https://mod-dotnet-bot.net");
var galleryUrl = new Uri(baseUrl, "gallery"); //$"{baseUrl}gallery/";
var doc = FetchHtmlDocument();
ParseHtml(doc);

HtmlDocument FetchHtmlDocument()
{
    var web = new HtmlWeb();
    var d = web.Load(galleryUrl);
    return d;
}

void ParseHtml(HtmlDocument document)
{
    var figs = document.DocumentNode.SelectNodes(@"//figure/img");

    foreach (var figure in figs)
    {
        string attr = figure.Attributes
            .FirstOrDefault()?.Value
                ?? throw new InvalidOperationException(nameof(ParseHtml));
        Uri fullFigureUrl = new Uri(baseUrl, attr); //$"{baseUrl}{attr}"
        Console.WriteLine(fullFigureUrl);
        
    }
}
