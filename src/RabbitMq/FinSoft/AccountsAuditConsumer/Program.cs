using AccountsAuditConsumer.RabbitMQ;
/*
 The Queue Processors layer where all the messages placed onto our queues will get processed.
 Accounts audit consumer responsible for receiving payment card and purchase order messages, and writing them to the accounting
 and auditing system by using what is called a wild card topic in RabbitMQ. 
 */
namespace AccountsAuditConsumer
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
