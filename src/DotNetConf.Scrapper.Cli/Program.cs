using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using DotNetConf.Scrapper.Cli.Core;

namespace DotNetConf.Scrapper.Cli
{
    class Program
    {
        static async Task Main(string[] args) =>
            await CommandLineParser.Create().InvokeAsync(args);
    }
}
