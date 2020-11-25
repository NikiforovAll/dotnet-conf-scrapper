using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace DotNetConf.Scrapper.Cli.Core
{
    public class BotExporter
    {
        public static async Task<string> ExportBot(Uri originUrl)
        {
            var tmpPath = Path.GetTempFileName();
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(originUrl);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.OpenWrite(tmpPath);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            return tmpPath;
        }
    }
}
