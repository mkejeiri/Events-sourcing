using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Persistence;

namespace eCommerce.Order
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;

    public class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "eCommerce.Order";
            LogManager.Use<DefaultFactory>()
                .Level(LogLevel.Info);

            var endpointConfiguration = new EndpointConfiguration(endpointName:"eCommerce.Order");
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo(errorQueue:"error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                // prevent the passing in of the controls thread context into the new
                // thread, which we don't need for sending a message
                .ConfigureAwait(continueOnCapturedContext:false);
            try
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            finally
            {
                await endpointInstance.Stop()
                    // prevent the passing in of the controls thread context into the new
                    // thread, which we don't need for sending a message
                    .ConfigureAwait(continueOnCapturedContext:false);
            }
        }
    }
}
