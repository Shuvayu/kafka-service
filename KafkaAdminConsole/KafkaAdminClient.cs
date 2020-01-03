using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KafkaAdminConsole
{
    internal class KafkaAdminClient
    {
        private readonly ILogger<KafkaAdminClient> _logger;
        private readonly IConfigurationRoot _config;

        public KafkaAdminClient(IConfigurationRoot config, ILogger<KafkaAdminClient> logger)
        {
            _logger = logger;
            _config = config;
        }
        internal async Task KafkaAdminstrationStartUp()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _config["EventHub:BootstrapServers"] }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                    new TopicSpecification { Name = _config["EventHub:GroupId"], ReplicationFactor = 1, NumPartitions = 1 } });
                    _logger.LogInformation($"Topic called {_config["EventHub: GroupId"]} created in Event Hub.");
                }
                catch (CreateTopicsException e)
                {
                    _logger.LogError($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }
        }
    }
}
