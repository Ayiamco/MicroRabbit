using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laundromat.SharedKernel.Core
{ 
    public abstract class MessageBrokerSubscriberBase : BackgroundService
    {
        protected IServiceProvider _sp;
        protected IModel channel;
        protected IConnection connection;


        public MessageBrokerSubscriberBase(IServiceProvider sp, string exchangeName, string exchangeType)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, exchangeType);
            _sp = sp;

        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
    }
        
}
