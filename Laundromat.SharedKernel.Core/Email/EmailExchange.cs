using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laundromat.SharedKernel.Core
{
    public class EmailExchange : IEmailExchange
    {
        private readonly IModel channel;
        public EmailExchange()
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare("direct-exchange-laundromat", ExchangeType.Direct);
        }

        public void PublishEmail(EmailMessage message)
        {
            using (channel)
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish("direct-exchange-laundromat", "email", null, body);
            }
            
        }

        public void ConsumeEmail(IEmailSender mailSender)
        {
            
            channel.QueueDeclare("direct-exchange-queue",
                durable: true, exclusive: false, autoDelete: false, arguments: null);

            //bind consumer to the  exchange
            channel.QueueBind("direct-exchange-queue", "direct-exchange-laundromat", "email");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                Console.WriteLine("message recieved");
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);
                mailSender.SendEmail(emailMessage);
            };

            channel.BasicConsume("direct-exchange-queue", true, consumer);
        }


    }
}
