using Laundromat.SharedKernel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Infrastructure
{
        public class LaundryConsumerService : BackgroundService
    {
        private IServiceProvider _sp;
        private ILaundryProfileExchange _exchange;


        public LaundryConsumerService(IServiceProvider sp, ILaundryProfileExchange exchange)
        {
            _sp = sp;
            _exchange = exchange;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // when the service is stopping
            // dispose these references
            // to prevent leaks
            if (stoppingToken.IsCancellationRequested)
            {
                _exchange.channel.Dispose();
                _exchange.connection.Dispose();
                return Task.CompletedTask;
            }

            Task.Run(() =>
            {
                // BackgroundService is a Singleton service
                // IAddLaundryRepo is declared a Scoped service
                // by definition a Scoped service can't be consumed inside a Singleton
                // to solve this, we create a custom scope inside the Singleton and 
                // perform the insertion.
                using (var scope = _sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<IAddLaundryRepo>();
                    _exchange.ConsumeCreateLaundry(db);

                }
            });
            
            return Task.CompletedTask;
        }
    }
}
  
