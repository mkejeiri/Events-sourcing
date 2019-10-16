namespace eCommerce.Monitoring
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;

    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "eCommerce.Monitoring";
            LogManager.Use<DefaultFactory>()
                .Level(LogLevel.Info);

            var endpointConfiguration = new EndpointConfiguration(endpointName: "eCommerce.Monitoring");
            endpointConfiguration.UseTransport<MsmqTransport>();

            //Step3: need to configure the endpoint to use JsonSerialization, since ServiceControl serializes using JSON.
            endpointConfiguration.UseSerialization<JsonSerializer>();

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo(errorQueue: "error");
            endpointConfiguration.AuditProcessedMessagesTo(auditQueue: "audit");

            //Step2: Since event classes are not marked with IEvent we need to enable unobtrusive mode. 
            //this convention tells  NServiceBus that all classes in the ServiceControl.Contracts
            //namespace are events (type of IEvent).
            endpointConfiguration.Conventions()
                .DefiningEventsAs(t =>
                    //ServiceControl.Contracts Assembly contains also other events (than MessageFailed)
                    //that let us respond to heartbeats that stop
                    //and restart, and custom checks that fail or succeed.
                    t.Namespace != null && t.Namespace.StartsWith("ServiceControl.Contracts"));

            //Step1: add routing in the config file, which registered subscribers with a ServiceControl endpoint.
            //Here a subscription is created for all events in the ServiceControl.Contracts assembly.
            //<add Assembly="ServiceControl.Contracts" Endpoint="Particular.ServiceControl"/>
            endpointConfiguration.CustomCheckPlugin(serviceControlQueue: "particular.servicecontrol");


            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                // prevent the passing in of the controls thread context into the new
                // thread, which we don't need for sending a message
                .ConfigureAwait(continueOnCapturedContext: false);
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
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
