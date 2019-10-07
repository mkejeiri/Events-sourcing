using PurchaseOrderConsumer.RabbitMQ;
/*
    The Queue Processors layer where all the messages placed onto our queues will get processed.
    Purchase order processors responsible for taking the standard asynchronous processing purchase orders. 
 */
namespace PurchaseOrderConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQConsumer client = new RabbitMQConsumer();
            client.CreateConnection();
            client.ProcessMessages();
            client.Close();
        }
    }
}
