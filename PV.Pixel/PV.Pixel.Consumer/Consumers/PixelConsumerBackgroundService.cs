using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PV.Pixel.Messages;
using System.Text.Json;

namespace PV.Pixel.Consumer.Consumers
{
    public class PixelConsumerBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IPixelHandlers _pixelHandlers;
        public PixelConsumerBackgroundService(
            IConfiguration configuration,
            IPixelHandlers pixelHandlers)
        {
            _configuration = configuration;
            _pixelHandlers = pixelHandlers;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellingToken)
        {
            var server = _configuration["Kafka:Server"];
            var defaultTopic = _configuration["Kafka:Default-Topic"];
            var defaultGroupId = _configuration["Kafka:Default-GroupId"];

            var kafkaConfig = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = defaultGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
            consumer.Subscribe(defaultTopic);

            while (!cancellingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellingToken);
                Console.WriteLine($"Received Partition: {consumeResult.Partition}, Offset: {consumeResult.Offset}");
                try
                {
                    var pixelRequested = JsonSerializer.Deserialize<PixelRequested>(consumeResult.Message.Value);
                    await _pixelHandlers.PixelRequestedHandler(pixelRequested);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
