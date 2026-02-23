namespace SmsService.Application.Interfaces;

public interface IMessageHandler<in TMessage>
{
    Task<Guid?> HandleAsync(TMessage message);
}