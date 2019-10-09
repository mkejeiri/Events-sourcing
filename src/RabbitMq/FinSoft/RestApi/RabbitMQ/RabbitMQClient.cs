using System;
using System.Collections.Generic;
using Payments.Models;
using RabbitMQ.Client;
/*
    RabbitMQ message broker where the API layer will post messages for processing
 */
namespace Payments.RabbitMQ
{
    public class RabbitMQClient
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        private const string ExchangeName = "Topic_Exchange";
        private const string CardPaymentQueueName = "CardPaymentTopic_Queue";
        private const string PurchaseOrderQueueName = "PurchaseOrderTopic_Queue";
        private const string AllQueueName = "AllTopic_Queue";

        public RabbitMQClient()
        {
            CreateConnection();
        }

        private static void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(exchange: ExchangeName, type: "topic");

            //queues will be persisted only created should the server ever get reset
            _model.QueueDeclare(queue: CardPaymentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _model.QueueDeclare(queue: PurchaseOrderQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _model.QueueDeclare(queue: AllQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            //also bind queues to the exchanges
            //Queue receive a copy of the message with payment.card, i.e. any message sent with
            //"payment.card" routing key will end up in the CardPaymentTopic_Queue queue
            _model.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName, routingKey: "payment.card");

            //Queue receive a copy of the message with payment.purchaseorder
            _model.QueueBind(queue: PurchaseOrderQueueName, exchange: ExchangeName, routingKey: "payment.purchaseorder");

            //Queue always receive a copy of the message regardless of whether it is a card or purchase order
            //message broker puts explicitly any received message with routing key that starts with **payment.** in the AllTopic_Queue and
            //we don't have to physically do it!!!=> accounts auditing department subscriber will receive a copy of all the messages
            _model.QueueBind(queue: AllQueueName, exchange: ExchangeName, routingKey: "payment.*");
        }

        public void Close()
        {
            _connection.Close();
        }

        public void SendPayment(CardPayment payment)
        {
            SendMessage(payment.Serialize(), "payment.card");
            Console.WriteLine(" Payment Sent {0}, £{1}", payment.CardNumber,
                payment.Amount);
        }

        public void SendPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            SendMessage(purchaseOrder.Serialize(), routingKey: "payment.purchaseorder");

            Console.WriteLine(" Purchase Order Sent {0}, £{1}, {2}, {3}",
                purchaseOrder.CompanyName, purchaseOrder.AmountToPay,
                purchaseOrder.PaymentDayTerms, purchaseOrder.PoNumber);
        }

        public void SendMessage(byte[] message, string routingKey)
        {
            _model.BasicPublish(exchange: ExchangeName, routingKey: routingKey, basicProperties: null, body: message);
        }
    }
}
