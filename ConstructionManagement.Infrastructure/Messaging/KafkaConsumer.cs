using Confluent.Kafka;
using ConstructionManagement.Domain.Entities;
using Elasticsearch.Net;
using Microsoft.Extensions.Hosting;
using Nest;
using Newtonsoft.Json;

namespace ConstructionManagement.Infrastructure.Messaging;

public class KafkaConsumer(IElasticClient elasticSearch) : IHostedService
{
    private readonly string _bootstrapServers = "localhost:9092";
    private readonly string _topic = "es.construction.hbx";
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Kafka Consumer Started...");
        _ = Task.Run(() => StartListening(_cancellationTokenSource.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StartListening(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "project-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                var project = JsonConvert.DeserializeObject<Project>(consumeResult.Message.Value);

                if (project != null)
                {
                    var response = await elasticSearch.IndexAsync(project, idx => idx
                        .Index("construction-projects") // ✅ Specify index name
                        .Refresh(Refresh.WaitFor), cancellationToken);

                    if (!response.IsValid)
                    {
                        Console.WriteLine($"Failed to index document: {response.DebugInformation}");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Kafka Consumer Stopping...");
        }
        finally
        {
            consumer.Close();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Kafka Consumer Stopped...");
        _cancellationTokenSource.Cancel(); // ✅ Stop the consumer
        return Task.CompletedTask;
    }
}