using RabbitMQ.Client;

namespace Laundromat.SharedKernel.Core
{
    public interface ILaundryProfileExchange
    {
        IModel channel { get; set; }
        IConnection connection { get; set; }
        void ConsumeCreateLaundry(IAddLaundryRepo laundryRepo);
        void PublichCreateLaundry(NewLaundryDto newLaundryDto);
    }
}