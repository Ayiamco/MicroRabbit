using Authentication.Interfaces;
using Authentication.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Infrastructure
{
    public class MessageBrokerClient : IMessageBrokerClient
    {
        private readonly ConnectionFactory factory;
        public MessageBrokerClient()
        {
            factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
        }

        public void PublishEmail(EmailMessage message)
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare("direct-exchange-laundromat", ExchangeType.Direct);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish("direct-exchange-laundromat", "email", null, body);
        }
    }
}
