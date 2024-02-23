using Confluent.Kafka;
using PV.Pixel.Messages;
using System.Text.Json;

namespace PV.Pixel.Service
{
    public interface IPixelProducer
    {
        Task<DeliveryResult<Null, string>> ProducePixelRequested(PixelRequested pixelRequested);
    }

    public class PixelProducer : IPixelProducer
    {
        private readonly string _defaultTopic;
        private readonly IProducer<Null, string> _producer;
        public PixelProducer(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:Server"],
            };
            _defaultTopic = configuration["Kafka:Default-Topic"]!;

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public Task<DeliveryResult<Null, string>> ProducePixelRequested(PixelRequested pixelRequested)
        {
            var message = new Message<Null, string> { Value = JsonSerializer.Serialize(pixelRequested) };
            return _producer.ProduceAsync(_defaultTopic, message);
        }
    }
}
