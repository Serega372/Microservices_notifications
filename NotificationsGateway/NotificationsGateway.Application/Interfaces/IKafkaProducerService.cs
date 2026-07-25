namespace NotificationsGateway.Application.Interfaces;

public interface IKafkaProducerService<in TMessage> : IDisposable
{
    Task ProduceAsync(TMessage message,  CancellationToken cancellationToken = default);
}