using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KafkaAdminConsole
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static async Task Main(string[] args)
        {
            // Create service collection
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<KafkaAdminClient>().KafkaAdminstrationStartUp();
            Console.ReadLine();
        }

        private static void ConfigureServices(ServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddLogging(configure => configure.AddConsole());

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configuration);

            serviceCollection.AddTransient<KafkaAdminClient>();
        }

        
    }
}
