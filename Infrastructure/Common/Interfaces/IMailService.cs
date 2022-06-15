using DN.WebApi.Infrastructure.DTOs.Mailing;

namespace DN.WebApi.Infrastructure.Common.Interfaces;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request);
}