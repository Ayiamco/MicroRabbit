using Laundromat.SharedKernel.Core.MessageBroker;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Laundromat.SharedKernel.Core
{
   public  class AddLaundryPublisher : MessageBrokerPublisherBase<NewLaundry>
   {
        public AddLaundryPublisher():base(BrokerExchangeNames.LaundryExchange,ExchangeType.Direct,BrokerRoutingKeys.AddLaundryKey)
        {

        } 
   }
}
