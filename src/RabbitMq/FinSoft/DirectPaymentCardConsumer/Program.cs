using DirectPaymentCardConsumer.RabbitMQ;
/*
     The Queue Processors layer where all the messages placed onto our queues will get processed.
     A direct payment processor that will make a payment and then return the result straight back to the user via the queue. 
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
