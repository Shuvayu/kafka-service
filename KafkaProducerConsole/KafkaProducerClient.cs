using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KafkaProducerConsole
{
    internal class KafkaProducerClient
    {
        private readonly ILogger<KafkaProducerClient> _logger;
        private readonly IConfigurationRoot _config;

        public KafkaProducerClient(IConfigurationRoot config, ILogger<KafkaProducerClient> logger)
        {
            _logger = logger;
            _config = config;
        }

        internal async Task StartPublisher()
        {
            string input = string.Empty;
            Console.WriteLine("To quit the seesion press q");
            Console.WriteLine("Send a message to the event hub.");
            while ((input = Console.ReadLine()) != "q")
            {
               await PublishToEventHub(input);
            }
            return;
        }

        private async Task PublishToEventHub(string message)
        {
            var config = new ProducerConfig { BootstrapServers = _config["EventHub:BootstrapServers"] };
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {               
                try
                {
                    var deliveryResult = await producer.ProduceAsync(_config["EventHub:GroupId"], new Message<Null, string> { Value = message });
                    _logger.LogInformation($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}
