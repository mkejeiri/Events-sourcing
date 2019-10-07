using PaymentCardConsumer.RabbitMQ;
/*
    The Queue Processors layer where all the messages placed onto our queues will get processed.
    Card payment responsible for taking the standard asynchronous card payments 
 */
namespace PaymentCardConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQConsumer client = new RabbitMQConsumer();
            client.CreateConnection();
            client.ProcessMessages();
        }
    }
}
