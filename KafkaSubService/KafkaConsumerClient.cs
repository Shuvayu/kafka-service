using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaSubService
{
    public class KafkaConsumerClient
    {
        private readonly ILogger<KafkaConsumerClient> _logger;
        private readonly IConfigurationRoot _config;

        public KafkaConsumerClient(IConfigurationRoot config, ILogger<KafkaConsumerClient> logger)
        {
            _logger = logger;
            _config = config;
        }

        public void StartConsumer()
        {
            Console.WriteLine("Reciveing messages from event hub.");

            ConsumerListener();
        }

        private void ConsumerListener()
        {
            var config = new ConsumerConfig
            {
                GroupId = _config["EventHub:GroupId"],
                BootstrapServers = _config["EventHub:BootstrapServers"],               
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_config["EventHub:GroupId"]);
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };
                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);
                            _logger.LogInformation($"Consumed message '{consumeResult.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }
    }
}
