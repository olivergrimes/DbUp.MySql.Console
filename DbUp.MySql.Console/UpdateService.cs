using DbUp.MySql.Console.Config;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;

namespace DbUp.MySql.Console
{
    public class UpdateService : IUpdateService
    {
        private readonly DatabaseConfig _config;

        public UpdateService(IOptions<DatabaseConfig> config)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        public bool Run()
        {
            if (string.IsNullOrWhiteSpace(_config.Connection))
            {
                WriteError("Please pass a connection string argument");

                return false;
            }

            var connectionStringBuilder = new MySqlConnectionStringBuilder(_config.Connection)
            {
                { "Allow User Variables", "True" }
            };

            var upgrader =
                DeployChanges.To
                .MySqlDatabase(connectionStringBuilder.ConnectionString)
                .WithScriptsFromFileSystem(_config.ScriptPath)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                WriteError(result.Error.ToString());
                return false;
            }

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Success!");
            System.Console.ResetColor();

            return true;
        }

        private static void WriteError(string error)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(error);
            System.Console.ResetColor();

#if DEBUG
            System.Console.ReadLine();
#endif
        }
    }
}
