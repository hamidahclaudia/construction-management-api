namespace ConstructionManagement.Application.Interfaces.Messaging;

public interface IKafkaProducer
{
    Task SendMessageAsync<T>(T message);
}