using System;
using RabbitMQ.Client;

namespace RabbitMQ.Examples
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;
                
        private const string QueueName = "StandardQueue_ExampleQueue";

        public static void Main()
        {            
            var payment1 = new Payment { AmountToPay = 25.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment2 = new Payment { AmountToPay = 5.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment3 = new Payment { AmountToPay = 2.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment4 = new Payment { AmountToPay = 17.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment5 = new Payment { AmountToPay = 300.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment6 = new Payment { AmountToPay = 350.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment7 = new Payment { AmountToPay = 295.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment8 = new Payment { AmountToPay = 5625.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment9 = new Payment { AmountToPay = 5.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
            var payment10 = new Payment { AmountToPay = 12.0m, CardNumber = "1234123412341234", Name = "Mr S Haunts" };
                        
            CreateQueue();            
                        
            SendMessage(payment1);
            SendMessage(payment2);
            SendMessage(payment3);
            SendMessage(payment4);
            SendMessage(payment5);
            SendMessage(payment6);
            SendMessage(payment7);
            SendMessage(payment8);
            SendMessage(payment9);
            SendMessage(payment10);
                                 
            Recieve();

            Console.ReadLine();
        }

        private static void CreateQueue()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest"};
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            //tells the broker the queue is durable. i.e. that queue is persisted to disk and will survive,
            //or be re-created when the server is restarted.
            _model.QueueDeclare(QueueName, durable:true, exclusive:false, autoDelete:false, arguments:null);            
        }

        private static void SendMessage(Payment payment)
        {            
            _model.BasicPublish("", QueueName, null, payment.Serialize());
            Console.WriteLine(" [x] Payment Message Sent : {0} : {1} : {2}", payment.CardNumber, payment.AmountToPay, payment.Name);            
        }

        public static void Recieve()
        {
            var consumer = new QueueingBasicConsumer(_model);            
            var msgCount = GetMessageCount(_model, QueueName);
            
            _model.BasicConsume(QueueName, true, consumer);
                    
            var count = 0;
            
            while (count < msgCount)
            {                               
                var message = (Payment)consumer.Queue.Dequeue().Body.DeSerialize(typeof(Payment));

                Console.WriteLine("----- Received {0} : {1} : {2}", message.CardNumber, message.AmountToPay, message.Name);
                count++;
            }                   
        }

        private static uint GetMessageCount(IModel channel, string queueName)
        {
            var results = channel.QueueDeclare(queueName, true, false, false, null);
            return results.MessageCount;                
        }
    }
}
