using Laundromat.SharedKernel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Infrastructure
{
        public class LaundryConsumerService : BackgroundService
    {
        private IServiceProvider _sp;
        private IModel  channel;


        public LaundryConsumerService(IServiceProvider sp)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare("LaundryProfileExchangeDirect", ExchangeType.Direct);
            _sp = sp;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // when the service is stopping
            // dispose these references
            // to prevent leaks
            if (stoppingToken.IsCancellationRequested)
            {
                channel.Dispose();
                //connection.Dispose();
                return Task.CompletedTask;
            }
            channel.QueueDeclare("LaundryProfileExchange-queue",
                durable: true, exclusive: false, autoDelete: false, arguments: null);

            //bind consumer to the  exchange
            channel.QueueBind("LaundryProfileExchange-queue", "LaundryProfileExchangeDirect", "laundryProfile");
            Console.WriteLine("Subscribing to the LaundryProfile Queue.");
            var consumer = new EventingBasicConsumer(channel);
            Task.Run(() =>
            {
                // BackgroundService is a Singleton service
                // IAddLaundryCommand is declared a Scoped service
                // by definition a Scoped service can't be consumed inside a Singleton
                // to solve this, we create a custom scope inside the Singleton and 
                // perform the insertion.
                
                consumer.Received += async (sender, e) =>
                {
                    Console.WriteLine("Laundry creation message recieved.");
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var newlaundryDto = JsonConvert.DeserializeObject<NewLaundryDto>(message);
                    using (var scope = _sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<IAddLaundryCommand>();
                        var laundryId = await db.AddLaundry(newlaundryDto);
                        Console.WriteLine($@"Laundry created :: Id:{laundryId} Name:{newlaundryDto.LaundryName}");
                    }
                    

                };
                
            });

            channel.BasicConsume("LaundryProfileExchange-queue", true, consumer);
            Console.WriteLine("Subscribing to the LaundryProfile Queue was successfull");
            return Task.CompletedTask;
        }
    }
}
  
