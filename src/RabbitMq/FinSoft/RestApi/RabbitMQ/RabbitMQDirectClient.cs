using System;
using System.Text;
using Payments.Models;
using RabbitMQ.Client;

namespace Payments.RabbitMQ
{
    public class RabbitMQDirectClient
    {
        private IConnection _connection;
        private IModel _channel;
        private string _replyQueueName;
        private QueueingBasicConsumer _consumer;

        public void CreateConnection()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _replyQueueName = _channel.QueueDeclare(queue: "rpc_reply", durable: true, exclusive: false, autoDelete: false, arguments: null);

            _consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(queue:_replyQueueName, noAck:true,consumer: _consumer);
        }

        public void Close()
        {
            _connection.Close();
        }

        public string MakePayment(CardPayment payment)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = corrId;

            _channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: payment.Serialize());
            //just sitting and wait for a response from the direct card payment consumer
            while (true)
            {
                var ea = _consumer.Queue.Dequeue();

                if (ea.BasicProperties.CorrelationId != corrId) continue;

                var authCode = Encoding.UTF8.GetString(ea.Body);
                return authCode;
            }
        }
    }
}
