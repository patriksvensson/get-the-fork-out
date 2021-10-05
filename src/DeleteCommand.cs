using Octokit;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetTheForkOut
{
    public sealed class DeleteCommand : AsyncCommand<DeleteCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<GITHUB-API-KEY>")]
            [Description(
                "The GitHub API key to use.\nThe API key must have the following permissions: " +
                "[blue]public_repo[/], [blue]delete:packages[/]")]
            public string ApiKey { get; }

            [CommandOption("--i-know-what-i-am-doing")]
            [DefaultValue(false)]
            [Description(
                "No prompts will be shown.\n" +
                "[yellow]WARNING:[/] All forks will automatically be deleted")]
            public bool Force { get; }

            public Settings(string apiKey, bool force)
            {
                ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
                Force = force;
            }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new GitHubClient(new ProductHeaderValue("Get-The-Fork-Out"));
            client.Credentials = new Credentials(settings.ApiKey);
            var user = await client.User.Current();

            var forks = await GetAllForks(client);
            if (!forks.Any())
            {
                AnsiConsole.MarkupLine($"No forks for user [yellow]{user.Login}[/] to delete.");
                return 0;
            }

            if (!ShowConfirmation(user, forks))
            {
                return -1;
            }

            foreach (var fork in forks)
            {
                if (!settings.Force)
                {
                    AnsiConsole.WriteLine();
                }

                // Show confirmation prompt
                if (ConfirmDelete(fork, settings))
                {
                    // Delete the fork
                    await client.Repository.Delete(fork.Owner.Login, fork.Name);
                    AnsiConsole.MarkupLine($"✅ Deleted [yellow]{fork.Owner.Login}[/]/[yellow]{fork.Name}[/]");
                }
            }

            return 0;
        }

        private static bool ShowConfirmation(User user, List<Repository> forks)
        {
            var table = new Table().Expand();
            table.AddColumns("Owner", "Repository", "Last updated");
            foreach (var fork in forks)
            {
                table.AddRow(
                    fork.Owner.Login.EscapeMarkup(),
                    fork.Name.EscapeMarkup(),
                    fork.UpdatedAt.ToString("G").EscapeMarkup());
            }

            AnsiConsole.Write(table);
            AnsiConsole.Write(new Panel(
                $"This tool is used to [red u b]DELETE[/] forks owned by " +
                $"the user [yellow]{user.Login}[/].\n" +
                "Deleted forks [b]cannot[/] be restored. " +
                "Press [white]CTRL+C[/] to abort at any time.")
                    .Header("[yellow]WARNING[/]")
                    .BorderColor(Color.Red)
                    .Expand());

            return AnsiConsole.Confirm("Are you sure that you want to continue?", defaultValue: false);
        }

        private static async Task<List<Repository>> GetAllForks(GitHubClient client)
        {
            var page = 1;
            var result = new List<Repository>();

            var user = await client.User.Current();
            var repositories = await AnsiConsole.Status().StartAsync("Retrieving forks...", async (status) =>
            {
                while (page < 10)
                {
                    var repositories = await client.Repository.GetAllForUser(
                        user.Login,
                        new ApiOptions
                        {
                            StartPage = page,
                            PageSize = 30,
                            PageCount = 1,
                        });

                    if (repositories.Count == 0)
                    {
                        break;
                    }

                    result.AddRange(repositories);
                    page++;
                }

                return result;
            });

            return repositories
                .Where(repo => repo.Fork)
                .OrderBy(repo => repo.UpdatedAt)
                .ToList();
        }

        private static bool ConfirmDelete(Repository fork, Settings settings)
        {
            if (settings.Force)
            {
                return true;
            }

            return AnsiConsole.Confirm($"Delete [yellow]{fork.Owner.Login}[/]/[yellow]{fork.Name}[/]", defaultValue: false);
        }
    }
}