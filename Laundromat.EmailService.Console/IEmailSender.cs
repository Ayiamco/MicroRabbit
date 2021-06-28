namespace Laundromat.EmailService.Console
{
    public interface IEmailSender
    {
        void SendEmail(Message message, bool IsHTML = false);
        Task SendEmailAsync(Message message, bool IsHTML = false);
    }
}