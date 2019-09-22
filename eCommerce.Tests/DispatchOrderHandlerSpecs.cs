using eCommerce.Dispatch;
using eCommerce.Order;
using eCommerce.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus.Testing;


namespace eCommerce.Tests
{
    [TestClass]
    public class DispatchOrderHandlerSpecs
    {
        [TestMethod]
        public void Send_DispatchOrderCommand_receive_IOrderDispatchedMessage()
        {
            Test.Handler<DispatchOrderHandler>()
                .ExpectReply<IOrderDispatchedMessage>(m => m == m)
                .OnMessage<DispatchOrderCommand>();
        }
    }
}
