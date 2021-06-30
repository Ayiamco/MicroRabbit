using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Laundromat.SharedKernel.Core
{
   public  class LaundryProfileExchange : ILaundryProfileExchange
    {
        public IModel channel { get; set; }
        public IConnection connection { get; set; }
        public LaundryProfileExchange()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare("LaundryProfileExchangeDirect", ExchangeType.Direct);

        }

        public void PublichCreateLaundry(NewLaundryDto newLaundryDto)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newLaundryDto));
            channel.BasicPublish("LaundryProfileExchangeDirect", "laundryProfile", null, body);

        }

        public void ConsumeCreateLaundry(IAddLaundryCommand laundryRepo)
        {

            channel.QueueDeclare("LaundryProfileExchange-queue",
                durable: true, exclusive: false, autoDelete: false, arguments: null);

            //bind consumer to the  exchange
            channel.QueueBind("LaundryProfileExchange-queue", "LaundryProfileExchangeDirect", "laundryProfile");
            Console.WriteLine("Subscribing to the LaundryProfile Queue.");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received +=  async (sender, e) =>
            {
                Console.WriteLine("Laundry creation message recieved.");
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var newlaundryDto = JsonConvert.DeserializeObject<NewLaundryDto>(message);
                var laundryId= await  laundryRepo.AddLaundry(newlaundryDto);
                Console.WriteLine($@"Laundry created :: Id:{laundryId} Name:{newlaundryDto.LaundryName}");

            };

            channel.BasicConsume("LaundryProfileExchange-queue", true, consumer);
            Console.WriteLine("Subscribing to the LaundryProfile Queue was successfull");
        }
    }
}
