using DbUp.MySql.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DbUp.MySql.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "--conn", "Database:Connection" },
                { "--path", "Database:ScriptPath" },
            };

            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true)
               .AddCommandLine(args, switchMappings)
               .Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<DatabaseConfig>(config.GetSection("Database"))
                .AddSingleton<IUpdateService, UpdateService>()
                .BuildServiceProvider();

            var updateService = serviceProvider.GetService<IUpdateService>();

            updateService.Run();
        }
    }
}
