using System.Threading.Tasks;

namespace DigitalLibrary.API.Services.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
