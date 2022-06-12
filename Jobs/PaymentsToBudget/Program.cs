using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Yoda.Application;

namespace PaymentsToBudget
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configuration) => {
                    configuration
                         .AddYodaJsonFiles("appsettings", context.HostingEnvironment.EnvironmentName)
                         .AddUserSecrets(Assembly.GetEntryAssembly(), true)
                         .AddEnvironmentVariables()
                         .AddCommandLine(args)
                         .UseVaultUnsecreter(context.HostingEnvironment.EnvironmentName);
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
