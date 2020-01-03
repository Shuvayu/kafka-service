using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaSubService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly KafkaConsumerClient _kafkaConsumerClient;

        public Worker(ILogger<Worker> logger, KafkaConsumerClient kafkaConsumerClient)
        {
            _logger = logger;
            _kafkaConsumerClient = kafkaConsumerClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _kafkaConsumerClient.StartConsumer();
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
