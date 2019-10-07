using System;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace DirectPaymentCardConsumer.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _channel;
        private static QueueingBasicConsumer _consumer;
        private static Random _rnd;

        public void ProcessMessages()
        {
            while (true)
            {
                GetMessageFromQueue();
            }
        }

        private void GetMessageFromQueue()
        {
            string response = null;
            var ea = _consumer.Queue.Dequeue();
            var props = ea.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            Console.WriteLine("----------------------------------------------------------");

            try
            {
                response = MakePayment(ea);
                Console.WriteLine("Correlation ID = {0}", props.CorrelationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ERROR : " + ex.Message);
                response = "";
            }
            finally
            {
                if (response != null)
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("");
        }

        private string MakePayment(BasicDeliverEventArgs ea)
        {
            var payment = (CardPayment)ea.Body.DeSerialize(typeof(CardPayment));
            var response = _rnd.Next(1000, 100000000).ToString(CultureInfo.InvariantCulture);
            Console.WriteLine("Payment -  {0} : £{1} : Auth Code <{2}> ", payment.CardNumber, payment.Amount, response);

            return response;
        }

        public void CreateConnection()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(queue: "rpc_queue", noAck: false, consumer: _consumer);
            _rnd = new Random();
        }

        public void Close()
        {
            _connection.Close();
        }
    }
}
