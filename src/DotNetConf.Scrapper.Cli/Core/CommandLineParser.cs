using System;
using System.Linq;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DotNetConf.Scrapper.Cli.Models;
using Spectre.Console;
using System.Threading.Tasks;

namespace DotNetConf.Scrapper.Cli.Core
{
    public class CommandLineParser
    {
        /// <summary>
        /// Creates a parser
        /// </summary>
        /// <remarks>
        /// Inspired by https://github.com/dotnet/interactive/blob/main/src/dotnet-interactive/CommandLine/CommandLineParser.cs
        /// </remarks>
        /// <returns></returns>
        public static Parser Create()
        {
            var root = new RootCommand()
            {

            };
            root.AddCommand(ListBots());
            root.AddCommand(DisplayRandomBot());
            var builder = new CommandLineBuilder(root)
                .UseDefaults();

            return builder.Build();

            /// <summary>
            /// Command to list a bots from gallery
            /// </summary>
            /// <returns></returns>
            static Command ListBots()
            {
                var listCommand = new Command("list")
                {
                    new Option<int>("--limit", "the number of bots to return")
                    {
                        IsRequired = false
                    },
                };
                listCommand.Handler =
                    CommandHandler.Create(static (int? limit) =>
                {
                    var scrapper = new BotScrapper();
                    var table = new Table();
                    table.AddColumn(new TableColumn(nameof(Robot.Name)).Centered());
                    table.AddColumn(nameof(Robot.Uri));
                    var botsQuery = scrapper.ListRobots();
                    botsQuery = limit.HasValue ? botsQuery.Take(limit.Value) : botsQuery;
                    botsQuery.ToList().ForEach(r => table.AddRow(r.Name, r.Uri.ToString()));
                    AnsiConsole.Render(table);
                });
                return listCommand;
            }

            /// <summary>
            /// Displays random bot
            /// </summary>
            /// <returns></returns>
            static Command DisplayRandomBot()
            {
                var randomCommand = new Command("random")
                {
                    new Option<int>("--size", getDefaultValue: () => 32, "specify maximum width"),
                    new Option<bool>("--render", "draw pixeled bot")
                };
                ICommandHandler commandHandler = CommandHandler.Create(async (int size, bool render) =>
                {
                    var scrapper = new BotScrapper();
                    var random = new Random();
                    List<Robot> bots = scrapper.ListRobots().ToList();
                    var randomBot = bots[random.Next(bots.Count)];
                    RenderBotHeader(randomBot);
                    await (render ? RenderBot(randomBot, size) : Task.CompletedTask);
                });
                randomCommand.Handler = commandHandler;

                static async Task RenderBot(Robot r, int size)
                {
                    Uri uri = r.Uri;
                    var localFile = await BotExporter.ExportBot(uri);
                    var image = new CanvasImage(localFile);
                    image.MaxWidth(size);
                    AnsiConsole.Render(new Panel(image));
                }

                static void RenderBotHeader(Robot r)
                {
                    Uri uri = r.Uri;
                    AnsiConsole.Render(
                        new Panel(new Markup($"[red dim]{uri}[/]"))
                            .Expand()
                            .AsciiBorder()
                            .Header($"[green]{r.Name}[/]")
                            .HeaderAlignment(Justify.Center));
                }
                return randomCommand;
            }
        }


    }
}
