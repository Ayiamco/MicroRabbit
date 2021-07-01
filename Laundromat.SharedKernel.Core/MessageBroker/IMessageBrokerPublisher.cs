namespace Laundromat.SharedKernel.Core
{
    public interface IMessageBrokerPublisher<T> where T : class
    {
        void PublishEvent(T message);
    }
}