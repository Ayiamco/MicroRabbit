using Laundromat.SharedKernel.Core;
using System.Threading.Tasks;

namespace Laundromat.SharedKernel.Core
{
    public interface IEmailSender
    {
        int SendEmail(EmailMessage message, bool IsHTML = false);
        Task SendEmailAsync(EmailMessage message, bool IsHTML = false);
    }
}