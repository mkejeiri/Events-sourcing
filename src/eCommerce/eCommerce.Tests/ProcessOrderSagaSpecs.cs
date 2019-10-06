using eCommerce.Messages;
using eCommerce.Saga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus.Testing;


namespace eCommerce.Tests
{
    [TestClass]
    public class ProcessOrderSagaSpecs
    {
        [TestMethod]
        public void Send_ProcessOrderCommand_when_PlanOrderCommand_sent()
        {
            Test.Saga<ProcessOrderSaga>()
                //Expected result first!
                .ExpectSend<PlanOrderCommand>()
                //is used when the message is a concrete type and not an interface.
                .When((saga, context) => saga.Handle(new ProcessOrderCommand(), context));
        }

        [TestMethod]
        public void Send_DispatchOrderCommand_when_OrderDispatchedMessage_received()
        {
            Test.Saga<ProcessOrderSaga>()
                //Expected result first!
                .ExpectReplyToOriginator<OrderProcessedMessage>()
                //WhenHandling is used when the message is an interface and not a concrete type.
                .WhenHandling<IOrderDispatchedMessage>() 
                .AssertSagaCompletionIs(true); //we all done we expect the saga to be complete
        }
    }
}
