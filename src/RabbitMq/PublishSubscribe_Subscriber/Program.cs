using System;
using RabbitMQ.Client;

namespace RabbitMQ.Examples
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static QueueingBasicConsumer _consumer;

        private const string ExchangeName = "PublishSubscribe_Exchange";

        static void Main()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    var queueName = DeclareAndBindQueueToExchange(channel);

                    //consumer has created its own queue, and subscribed itself to the exchange
                    //Now it will receive all messages that are sent to that exchange ("PublishSubscribe_Exchange")
                    //noAck: true =>  No waiting for a message acknowledgement before receiving the next message.
                    //We don't need to as our subscriber application is reading from its own queue ()
                    channel.BasicConsume(queue: queueName, noAck: true, consumer: _consumer);

                    while (true)
                    {
                        var ea = _consumer.Queue.Dequeue();
                        var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                        //no need to send message acknowledgement to tell RabbitMQ that we're finished with a message, 
                        //because we want all messages to be sent to every consumer, otherwise get removed from the queue!
                        //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        Console.WriteLine("----- Payment Processed {0} : {1}", message.CardNumber, message.AmountToPay);
                    }
                }
            }
        }


        private static string DeclareAndBindQueueToExchange(IModel channel)
        {

            //Idempotent operation : if the exchange is already there, then nothing will happen, otherwise it will get created.
            channel.ExchangeDeclare(exchange: ExchangeName, type: "fanout");

            //This uses a system generated queue name such as amq.gen-qVC1KT9w-plxzpV9MVId9w
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "");
            _consumer = new QueueingBasicConsumer(channel);
            return queueName;
        }
    }
}
