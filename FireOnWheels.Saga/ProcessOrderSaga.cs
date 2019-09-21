using System;
using System.Threading.Tasks;
using FireOnWheels.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace FireOnWheels.Saga
{
    /*
     NServiceBus know what saga belongs to what messages through an abstract method in a Saga class : i.e. ConfigureHowToFindSaga, we specify 
     all messages that are received by the saga and the data object that the saga has persisted. ConfigureHowToFindSaga provides a SagaPropertyMapper object as a parameter
     generic method, with ConfigureMapping as the generic parameters supply the message type that you want to map to the SagaData object. With a lambda,
     we instruct NServiceBus which property to use in the message for the mapping. With ToSaga, we supply the property to use on the other side in the SagaData object.
     */
    public class ProcessOrderSaga : Saga<ProcessOrderSagaData>,
        IAmStartedByMessages<ProcessOrderCommand>,
        IHandleMessages<IOrderPlannedMessage>,
        IHandleMessages<IOrderDispatchMessage>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ProcessOrderSaga));

        //Explicite implementation
        //Task IHandleMessages<ProcessOrderCommand>.Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        //{
        //    throw new System.NotImplementedException();
        //}
        public async Task Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        {
            logger.Info($"ProcessOrder command received. Starting saga for orderId  {message.OrderId}");

            //Copy all the data into the Saga
            Data.OrderId = message.OrderId;
            Data.Price = message.Price;
            Data.AddressFrom = message.AddressFrom;
            Data.AddressTo = message.AddressTo;
            Data.Weight = message.Weight;

            //We send the PlanOrderCommand routed in the app.config file to the input queue of the planning service
            //We only fill the message with the data it needs. The planning service is a
            //commandline application hosting an endpoint, and in its handler We do the planning work.
            await context.Send(new PlanOrderCommand { OrderId = Data.OrderId, AddressTo = Data.AddressTo })
                .ConfigureAwait(false);


        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ProcessOrderSagaData> mapper)
        {
            //Select s.OrderId from ProcessOrderSagaData s where s.OrderId = message.OrderId (i.e. ProcessOrderCommand.OrderId )
            mapper.ConfigureMapping<ProcessOrderCommand>(
                    msg => msg.OrderId //ProcessOrderCommand part
                    )
                .ToSaga(
                    s => s.OrderId //ProcessOrderSagaData part
                    );
        }

        /*
         There is neither a mapping in the ConfigureHowToFindSaga nor IOrderPlannedMessage, nor routing in the app.config for 
         IOrderPlannedMessage message. we use a reply, thus NServiceBus handles this for us.
         */
        public async Task Handle(IOrderPlannedMessage message, IMessageHandlerContext context)
        {
            //We send the DispatchOrderCommand. This time it's routed in the app.config to the Dispatch service.
            //The message only contains what Dispatch needs to know.
                        logger.Info($"Order {Data.OrderId} has been planned. Sending dispatch command.");
            await context.Send(new DispatchOrderCommand { AddressTo = Data.AddressTo, Weight = Data.Weight });
        }

        public async Task Handle(IOrderDispatchMessage message, IMessageHandlerContext context)
        {
           logger.Info($"Order {Data.OrderId} has been dispatched. Notifying originator and ending Saga...");

            //When the IOrderDispatchedMessage comes back we want to let the application that causes saga to instantiate 
            //know that the order has been processed, so we use the ReplyToOriginator method of the saga (no routing needed!)
            await ReplyToOriginator(context, new OrderProcessedMessage()).ConfigureAwait(false);

            //tell the saga it's done with the MarkAscomplete method
            //The saga will throw away the data object in the configured storage
            MarkAsComplete();
        }
    }

}
