namespace eCommerce.OrderActivity
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;

    public class Program
    {
        static void Main()
        {
            ListenToOrderActivityAsync().GetAwaiter().GetResult();
        }

        //This will listen to all order activities IOrderActivityEvent message 
        //and handles them through OrderActivityHandler
        static async Task ListenToOrderActivityAsync()
        {
            Console.Title = "eCommerce.OrderActivity";
            LogManager.Use<DefaultFactory>().Level(LogLevel.Info);

            var endpointConfiguration = new EndpointConfiguration(endpointName: "eCommerce.OrderActivity");
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo(errorQueue: "error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                // prevent the passing in of the controls thread context into the new
                // thread, which we don't need for sending a message
                .ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                Console.WriteLine("eCommerce.OrderActivity. Press any key to exit");
                Console.ReadKey();
            }
            finally
            {
                await endpointInstance.Stop()
                    // prevent the passing in of the controls thread context into the new
                    // thread, which we don't need for sending a message
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
