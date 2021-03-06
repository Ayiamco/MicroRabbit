using AutoMapper;
using Laundromat.MainProfile.API.RequestModels.CommandRequests;
using Laundromat.MainProfile.API.ResponseModels;
using Laundromat.SharedKernel.Core;
using Laundromat.SharedKernel.Core.MessageBroker;
using MediatR;
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

namespace Laundromat.MainProfile.API.BackgroudWorkers
{
    public class LaundryAddedSubscriber : MessageBrokerSubscriberBase
    {
        public LaundryAddedSubscriber(IServiceProvider sp)
            :base(sp, BrokerExchangeNames.LaundryExchange, ExchangeType.Direct)
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // when the service is stopping
            // dispose these references
            // to prevent leaks
            if (stoppingToken.IsCancellationRequested)
            {
                channel.Dispose();
                connection.Dispose();
                return Task.CompletedTask;
            }

            var queueName = "LaundryProfileExchange-queue";
            channel.QueueDeclare(queueName,
                durable: true, exclusive: false, autoDelete: false, arguments: null);

            //bind consumer to the  exchange
            channel.QueueBind(queueName,BrokerExchangeNames.LaundryExchange, BrokerRoutingKeys.AddLaundryKey);
            Console.WriteLine($"Subscribing to the {BrokerExchangeNames.LaundryExchange} exchange.");
            var consumer = new EventingBasicConsumer(channel);
            Task.Run(() =>
            {
                // BackgroundService is a Singleton service a Scoped service can't be consumed inside a Singleton
                // to solve this, we create a custom scope inside the Singleton and perform the insertion.
                
                consumer.Received += async (sender, e) =>
                {
                    Console.WriteLine($"Laundry creation message recieved from {e.Exchange} exchange");
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var newlaundryDto = JsonConvert.DeserializeObject<NewLaundry>(message);
                    if(!string.IsNullOrEmpty(newlaundryDto.LaundryName) && !string.IsNullOrEmpty(newlaundryDto.OwnerName))
                    {
                        using (var scope = _sp.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                            var requestModel = mapper.Map<AddLaundryRequestModel>(newlaundryDto);
                            var mediatorResponse = await mediator.Send(requestModel);
                            if (mediatorResponse.Status == HandlerResponseStatus.Succeeded)
                                Console.WriteLine($@"Laundry created :: Id:{mediatorResponse.Data.Data} Name:{newlaundryDto.LaundryName}");

                            else
                                Console.WriteLine($@"Laundry created failure :: Id:{mediatorResponse.Data.Data} Name:{newlaundryDto.LaundryName}");
                        }
                    }
                    channel.BasicAck(1, false);
                    

                };
                
            });

            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("Subscribing to the LaundryProfile Queue was successfull");
            return Task.CompletedTask;
        }
    }
}
  
