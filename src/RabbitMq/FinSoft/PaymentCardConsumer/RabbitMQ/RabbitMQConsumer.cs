using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace PaymentCardConsumer.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;

        private const string ExchangeName = "Topic_Exchange";
        private const string CardPaymentQueueName = "CardPaymentTopic_Queue";

        public void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public void Close()
        {
            _connection.Close();
        }

        public void ProcessMessages()
        {
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    Console.WriteLine("Listening for Topic <payment.cardpayment>");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(exchange: ExchangeName, type: "topic");
                    channel.QueueDeclare(queue: CardPaymentQueueName,
                        durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName,
                        routingKey: "payment.cardpayment");

                    //the basic Qos with a prefix count of one.
                    //This means a consumer will reserve one message after queue to process at a time
                    //If any of those messages are not acknowledged when they are finished processing, then they will
                    //be put back onto the queue ready for another consumer to process them
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

                    //Subscription is a high level abstraction that has a more natural iterator feel to it.
                    //To use it we simply create a new instance and supply the channel
                    //and the queue which we want to get the messages from.
                    Subscription subscription = new Subscription(model: channel,
                        queueName: CardPaymentQueueName, noAck: false);

                    while (true)
                    {
                        //we enter a while called next and the subscriptions get the next message
                        BasicDeliverEventArgs deliveryArguments = subscription.Next();

                        //The serialized message throughout the card payment or (a purchase order if it is a purchase order consumer).
                        var message =
                            (CardPayment)deliveryArguments.Body.DeSerialize(typeof(CardPayment));

                        var routingKey = deliveryArguments.RoutingKey;

                        Console.WriteLine("--- Payment - Routing Key <{0}> : {1} : {2}", routingKey, message.CardNumber, message.Amount);

                        // Once we have finished with the message,we call the ACK method and the subscription
                        // to acknowledge the message
                        subscription.Ack(deliveryArguments);
                    }
                }
            }
        }
    }
}
