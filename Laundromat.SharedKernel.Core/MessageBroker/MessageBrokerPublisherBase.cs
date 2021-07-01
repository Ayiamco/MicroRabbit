using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laundromat.SharedKernel.Core
{
    public abstract class MessageBrokerPublisherBase<T> : IMessageBrokerPublisher<T> where T : class
    {
        protected IModel channel { get; set; }
        private string routingKey;
        private string exchangeName;
        public MessageBrokerPublisherBase(string exchangeName, string exchangeType,string routingKey)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, exchangeType);
            this.exchangeName = exchangeName;
            this.routingKey = routingKey;

        }

        public void PublishEvent(T message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchangeName, routingKey, null, body);

        }


    }
}
