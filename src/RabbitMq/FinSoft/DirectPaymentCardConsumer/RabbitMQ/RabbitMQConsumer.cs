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
            while (true) //run an infinite loop to get messages from the queue
            {
                GetMessageFromQueue();
            }
        }

        /// <summary>
        /// it de-queues the messages from the queue where I've
        /// already called consume, and we extract the properties
        /// </summary>
        private void GetMessageFromQueue()
        {
            string response = null;

            //de-queue our message and extract the basic properties from it
            //extract the correlation ID GUID that we generated in the web API client
            var ea = _consumer.Queue.Dequeue();
            var props = ea.BasicProperties;


            var replyProps = _channel.CreateBasicProperties();

            //attach correlation ID to the properties for the reply message
            //this correlation ID is what's used by the client to enforce
            //that this is a correct reply for the original message as an integrity check
            replyProps.CorrelationId = props.CorrelationId;

            Console.WriteLine("----------------------------------------------------------");

            try
            {
                //MakePayment deserializes the payment message and generates an off code random number
                //which will get sent back with our reply
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
                    //When we are ready to send our reply, we encode the off code as a byte array
                    //and post it onto the reply queue along with the message properties that
                    //contains our correlation ID 
                    /*
                     turn our reply which is going to be an authorization code into a byte array, 
                     and we then publish it to our reply queue
                     */
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
                //Reply has been sent with an acknowledged message
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

            //declare the RPC queue in case it hasn't already been declared
            _channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            //the basic Qos with a prefix count of one.
            //This means a consumer will reserve one message after queue to process at a time
            //If any of those messages are not acknowledged when they are finished processing, then they will
            //be put back onto the queue ready for another consumer to process them
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            //start consuming 
            _consumer = new QueueingBasicConsumer(_channel);
            //noAck: false => consumer will acknowledge when it finishes processing
            _channel.BasicConsume(queue: "rpc_queue", noAck: false, consumer: _consumer);

            //simple random number generator to generate our off code to send back for the payments
            _rnd = new Random();
        }

        public void Close()
        {
            _connection.Close();
        }
    }
}
