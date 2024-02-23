// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using PV.Pixel.Messages;
using System.Text;
using System.Text.Json;


var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

var server = config["Kafka:Server"];
var defaultTopic = config["Kafka:Default-Topic"];
var defaultGroupId = config["Kafka:Default-GroupId"];

var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), config["LogFile"]!);
var logDirectory = Path.GetDirectoryName(logFilePath);
if (!string.IsNullOrEmpty(logDirectory)) Directory.CreateDirectory(logDirectory);

var kafkaConfig = new ConsumerConfig
{
    BootstrapServers = server,
    GroupId = defaultGroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var cancellationTokenSource = new CancellationTokenSource();
using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
consumer.Subscribe(defaultTopic);

var thread = new Thread(() =>
{
    try
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(cancellationTokenSource.Token);
            Console.WriteLine($"Received Partition: {consumeResult.Partition}, Offset: {consumeResult.Offset}");
            try
            {
                var pixelRequested = JsonSerializer.Deserialize<PixelRequested>(consumeResult.Message.Value);
                Console.WriteLine($"Received message: {pixelRequested}");
                if (pixelRequested != null)
                {
                    using StreamWriter streamwriter = new(logFilePath, true, Encoding.UTF8, 65536);
                    var logEntry = $"{pixelRequested.RequestedOn:o}|{pixelRequested.Referer ?? "null"}|{pixelRequested.UserAgent ?? "null"}|{pixelRequested.VisitorIp}";
                    streamwriter.WriteLine(logEntry);
                    Console.WriteLine($"log:{logEntry}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"Consumer thread canceled.");
    }
});
thread.Start();

Console.WriteLine("Press any key to stop...");
Console.ReadKey();
cancellationTokenSource.Cancel();
thread.Join();
consumer.Close();