using EmailService.Core.Entities;

namespace EmailService.Core.Interfaces;

public interface IEmailsRepository
{
    public Task<Guid?> CreateAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}