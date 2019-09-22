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

            var endpointConfiguration = new EndpointConfiguration("eCommerce.OrderActivity");
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo("error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            try
            {
                Console.WriteLine("eCommerce.OrderActivity. Press any key to exit");
                Console.ReadKey();
            }
            finally
            {
                await endpointInstance.Stop()
                    .ConfigureAwait(false);
            }
        }
    }
}
