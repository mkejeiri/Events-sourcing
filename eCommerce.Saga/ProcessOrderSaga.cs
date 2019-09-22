using System;
using System.Threading.Tasks;
using eCommerce.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace eCommerce.Saga
{
    /*
     NO NEED FOR ROUTING AND ConfigureHowToFindSaga CASE:
     ----------------------------------------------------
     - Service will reply directly to the saga and NServiceBus knows where to send message (reply), 
        because the saga details are invisibly present in the message. 

     - in case ReplyToOriginator there is no need to specify a mapping for the message and configure how to find the saga.
       The Data abstract class contains the adverse of the originator (i.e. service) that started the saga. 
       using the ReplyToOriginator method in the saga class, a reply directly sent to the originator without 
       the need for routing config.
     */

    /*
     NServiceBus know what saga belongs to what messages through an abstract method in a Saga class : i.e. ConfigureHowToFindSaga, we specify 
     all messages that are received by the saga and the data object that the saga has persisted. ConfigureHowToFindSaga provides a SagaPropertyMapper object as a parameter
     generic method, with ConfigureMapping as the generic parameters supply the message type that you want to map to the SagaData object. With a lambda,
     we instruct NServiceBus which property to use in the message for the mapping. With ToSaga, we supply the property to use on the other side in the SagaData object.
     */
    public class ProcessOrderSaga : Saga<ProcessOrderSagaData>,
        IAmStartedByMessages<ProcessOrderCommand>, //a new Saga is started when ProcessOrderCommand message arrives from originator (RestApi/WebMVC Client),
                                                   //its implementation below : i.e. Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        IHandleMessages<IOrderPlannedMessage>,
        IHandleMessages<IOrderDispatchedMessage>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ProcessOrderSaga));

        //configure how NService bus find the saga data storage using mapping between received command (ProcessOrderCommand) and ProcessOrderSagaData storage
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ProcessOrderSagaData> mapper)
        {
            //Select s.OrderId from ProcessOrderSagaData s where s.OrderId = message.OrderId (i.e. ProcessOrderCommand.OrderId )
            //Read the OrderId property from ProcessOrderCommand and matched with OrderId property from the saga data store
            mapper.ConfigureMapping<ProcessOrderCommand>(
                    msg => msg.OrderId //ProcessOrderCommand part
                )
                .ToSaga(
                    s => s.OrderId //ProcessOrderSagaData part
                );
        }


        //Explicite implementation
        //Task IHandleMessages<ProcessOrderCommand>.Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        //{
        //    throw new System.NotImplementedException();
        //}


        //Step 1 : the saga receive ProcessOrderCommand  from Originator (RestApi/WebMVC Client) 
        //the saga send Command (PlanOrderCommand) to planner service (see config file for endpoint)
        //the saga expects to receive IOrderPlannedMessage (see planner service),
        //see above the received order :  IHandleMessages<IOrderPlannedMessage> and then IHandleMessages<IOrderDispatchedMessage>
        public async Task Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"ProcessOrder command received. Starting saga for orderId  {message.OrderId}");
            logger.Info($"ProcessOrder command received. Starting saga for orderId  {message.OrderId}");

            //Copy all the data into the Saga
            Data.OrderId = message.OrderId;
            Data.Price = message.Price;
            Data.AddressFrom = message.AddressFrom;
            Data.AddressTo = message.AddressTo;
            Data.Weight = message.Weight;

            //the saga the PlanOrderCommand routed in the app.config file to the input queue of the planner service (eCommerce.Planner)
            //the saga only fill the message with the data eCommerce.Planner needs. The planning service is a
            //commandline application hosting an endpoint, and the saga delegates the planning work (the heavy lifting) to its handler.
            await context.Send(new PlanOrderCommand { OrderId = Data.OrderId, AddressTo = Data.AddressTo })
                .ConfigureAwait(false);
        }


        /*
         There is neither a mapping in the ConfigureHowToFindSaga nor IOrderPlannedMessage, nor routing in the app.config for 
         IOrderPlannedMessage message.  the saga use a reply, the NServiceBus handles this for us because it no routing needed (already exits as a hidden info in the context).
         */
        //Step 2 : the saga receives IOrderPlannedMessage from the planner service and then a DispatchOrderCommand sent to Order service
        public async Task Handle(IOrderPlannedMessage message, IMessageHandlerContext context)
        {
            //We send the DispatchOrderCommand. This time it's routed in the app.config, i.e. to eCommerce.Order service.
            //The message only contains what Dispatch needs to know.
            logger.Info($"Order {Data.OrderId} has been planned. Sending dispatch command.");
            await context.Send(new DispatchOrderCommand { AddressTo = Data.AddressTo, Weight = Data.Weight });
        }

        //Step 3: the saga receive a IOrderDispatchedMessage reply from eCommerce.Order service.
        //Step 4 : the saga send OrderProcessedMessage as a reply to Originator (RestApi or WebUI)  and we mark itself as completed
        public async Task Handle(IOrderDispatchedMessage message, IMessageHandlerContext context)
        {
           logger.Info($"Order {Data.OrderId} has been dispatched. Notifying originator and ending Saga...");

            //When the IOrderDispatchedMessage comes back, we want to let the APPLICATION that causes saga to instantiate 
            //KNOW that the order has been processed => so we use the ReplyToOriginator method of the saga (no routing needed!)
            await ReplyToOriginator(context, new OrderProcessedMessage()
            {
                AddressTo = Data.AddressTo,
                AddressFrom = Data.AddressFrom,
                Price = Data.Price,
                Weight = Data.Weight
            }).ConfigureAwait(false);

            //tell the saga it's done with the MarkAscomplete method
            //The saga will throw away the data object in the configured storage
            MarkAsComplete();
        }
    }

}
