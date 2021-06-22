using Authentication.Models;

namespace Authentication.Interfaces
{
    public interface IMessageBrokerClient
    {
        void PublishEmail(EmailMessage message);
    }
}