namespace Laundromat.SharedKernel.Core
{
    public interface IEmailExchange
    {
        void ConsumeEmail(IEmailSender mailSender);
        void PublishEmail(EmailMessage message);
    }
}