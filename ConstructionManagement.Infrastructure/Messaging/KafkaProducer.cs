using Confluent.Kafka;
using ConstructionManagement.Application.Interfaces.Messaging;
using Newtonsoft.Json;

namespace ConstructionManagement.Infrastructure.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly string _bootstrapServers = "localhost:9092";
    private readonly string _topic = "es.construction.hbx";

    public async Task SendMessageAsync<T>(T message)
    {
        var config = new ProducerConfig{ BootstrapServers = _bootstrapServers };
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        
        var jsonMessage = JsonConvert.SerializeObject(message);
        await producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
    }
}