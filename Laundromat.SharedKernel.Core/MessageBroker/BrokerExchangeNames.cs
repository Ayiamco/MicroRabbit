using System;
using System.Collections.Generic;
using System.Text;

namespace Laundromat.SharedKernel.Core.MessageBroker
{
    public static class BrokerExchangeNames
    {
        public const string LaundryExchange = "LaundryExchange";
    }

    public static class BrokerRoutingKeys
    {
        public static string AddLaundryKey { get; set; } = "AddLaundryRoutingKey";
    }
}
