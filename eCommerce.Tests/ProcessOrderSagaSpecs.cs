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
                .ExpectSend<PlanOrderCommand>()
                .When((saga, context) => saga.Handle(new ProcessOrderCommand(), context));
        }

        [TestMethod]
        public void ProcessOrderSaga_SendDispatchOrderCommand_WhenOrderDispatchedMessageReceived()
        {
            Test.Saga<ProcessOrderSaga>()
                .ExpectReplyToOriginator<OrderProcessedMessage>()
                .WhenHandling<IOrderDispatchedMessage>()
                .AssertSagaCompletionIs(true);
        }
    }
}
