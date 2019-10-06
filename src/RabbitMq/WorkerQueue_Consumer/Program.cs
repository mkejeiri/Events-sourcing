using System;
using RabbitMQ.Client;

namespace RabbitMQ.Examples
{
    public class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;

        private const string QueueName = "WorkerQueue_Queue";

        static void Main()
        {
            Receive();

            Console.ReadLine();
        }

        public static void Receive()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    //QueueDeclare is idempotent, i.e. it will only be created if it doesn't already exist.
                    channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //(Spec method) Configures Quality Of Service parameters of the Basic content-class.
                    //prefetchCount: 1 means that RabbitMQ won't dispatch a new message to a consumer, until
                    //that consumer is finished processing and acknowledged the message,
                    //RabbitMQ will dispatch a message on the next worker that is not busy 
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new QueueingBasicConsumer(channel);
                    //we want to expect an acknowledgement message
                    channel.BasicConsume(queue: QueueName, noAck: false, consumer: consumer);

                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();

                        //once we have the message, and have acted on it, we will send a delivery acknowledgement next
                        var message = (Payment)ea.Body.DeSerialize(typeof(Payment));

                        //This tells the message broker that we are finished processing the message,
                        //and we are ready to start processing the next message when it is ready.
                        //the next message will not be received by this consumer, until it sends this delivery acknowledgement. 
                        //acknowledgement sent to the RabbitMQ server, meaning we've finished with that message, and it can discard it from the queue
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        Console.WriteLine("----- Payment Processed {0} : {1}", message.CardNumber, message.AmountToPay);
                    }
                }
            }
        }
    }
}
