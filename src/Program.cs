using GetTheForkOut;
using Spectre.Console.Cli;

namespace GetTheForkOut
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<DeleteCommand>();
            app.Configure(config =>
            {
                config.SetApplicationName("dotnet get-the-fork-out");
            });

            return app.Run(args);
        }
    }
}
