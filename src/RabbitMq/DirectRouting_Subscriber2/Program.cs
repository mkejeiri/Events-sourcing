using System;
using RabbitMQ.Client;

namespace RabbitMQ.Examples
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;

        private const string ExchangeName = "DirectRouting_Exchange";
        private const string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";

        static void Main()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    //Queue binding to exchange and listen to PurchaseOrder messages
                    //Queues an exchanges are idempotent
                    channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
                    channel.QueueDeclare(queue: PurchaseOrderQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: PurchaseOrderQueueName, exchange: ExchangeName, routingKey: "PurchaseOrder");

                    //tells RabbitMQ to give one message at time per worker,
                    //i.e.  don't dispatch any message to a worker until it has processed and acknowledged the previous one.
                    //otherwise it will dispatch it to the next worker that is not busy
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


                    //queuing basic consumer is created 
                    var consumer = new QueueingBasicConsumer(channel);

                    //and basic consumer is called to start reading from the queue
                    //noAck: false => we care that the messages are safe on the queue and we want the message to be acknowledged
                    //in case of the consumer crashes, the message is put back into the queue and eventually later
                    //dispatched to the next idle worker.
                    //in case of the consumer succeeded, a Ack is sent back to the broker, message (successfully processed) is discarded 
                    //from the queue and worker is ready to process another one.
                    channel.BasicConsume(queue: PurchaseOrderQueueName, noAck: false, consumer: consumer);

                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();
                        var message = (PurchaseOrder)ea.Body.DeSerialize(typeof(PurchaseOrder));
                        var routingKey = ea.RoutingKey;

                        // a Ack is sent back to the broker, message (successfully processed) is discarded 
                        //from the queue and worker is ready to process another one.
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        Console.WriteLine("-- Purchase Order - Routing Key <{0}> : {1}, ExampleQueue{2}, {3}, {4}", routingKey, message.CompanyName, message.AmountToPay, message.PaymentDayTerms, message.PoNumber);
                    }
                }
            }
        }
    }
}
