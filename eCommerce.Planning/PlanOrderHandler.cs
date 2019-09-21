using System;
using System.Threading.Tasks;
using eCommerce.Messages;
using NServiceBus;

namespace eCommerce.Planning
{
    public class PlanOrderHandler: IHandleMessages<PlanOrderCommand>
    {
        //DO the planning work inside the handling
        public async Task Handle(PlanOrderCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"OrderId {message.OrderId} planned");
            //DO e.g : store the order in its own data store, and supply a web interface for the planner to work with.
            //Sent back to the Saga where the PlannedOrder message came from
            await context.Reply<IOrderPlannedMessage>(msg => { })
                /*
                  When the planning is done, we invoke reply method on the context object with an IOrderPlannedMessage, 
                  which will send it to the saga the planned order command message came from. IOrderPlanMessage doesn't 
                  have any properties It is just used to signal the saga it's done. The saga has all the data about the 
                  order anyway, so only if new data was introduced by the planning process, it has to be sent back to the saga
                 */
                .ConfigureAwait(false);
        }
    }
}
