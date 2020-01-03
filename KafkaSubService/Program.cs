using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaSubService
{
    public class Program
    {
        public static IConfigurationRoot configuration;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Build configuration
                    configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                        .AddJsonFile("appsettings.json", false)
                        .Build();

                    // Add access to generic IConfigurationRoot
                    services.AddSingleton(configuration);

                    services.AddHostedService<Worker>();
                    services.AddTransient<KafkaConsumerClient>();

                    // Add logging
                    //services.AddLogging(configure => configure.AddConsole());                    
                });
    }
}
