# I) RabbitMQ
Cross-platform system open source messaging system, integrate applications together by using exchanges and queues (Advanced Message Queue or AMQP protocol). RabbitMQ server is written in the Erlang programming language (designed for telecoms industry by Ericsson), Erlang supports distributed full torrent applications, thus ideal to build a messaging system, it has client libraries
that support many different programming environments (. NET, JAVA, Erlang, Ruby, Python,PHP, Perl, C, and C++, Node.js...)
 
The **RabbitMQ** server is a message broker that acts as a message coordinator for the applications that integrate together. i.e. a common platform for sending and receiving messages.

**RabbitMQ features** :
- reliability : message broker built on top of a solid, high-performance, reliable, and durable foundations, this includes Messages can be persisted to disk to guard from loss when a server is restarted, and message delivery acknowledgements from receiver to sender to ensure that the message has been received and stored.


- routing: messages passes through exchanges before they are stored in a queue, There are different exchange - a complex routing scenarios by binding exchanges together


- clustering and high availability: several servers together on a local network, which forms a single, logical message broker. Queues can also be mirrored across multiple servers in a cluster so that in the event of a server failure, we won't lose any messages.

- management web user interface: manage users and their permissions, exchanges, and queues.

- command line interface: offer the same level of administration as the web user interface, we can incorporate RabbitMQ administration into scripts

> paid support plan with a company called Pivotal, who runs and maintains the RabbitMQ open source project


### MSMQ (Microsoft platform since 1997)
Messaging protocol that allows applications running on separate servers and processes to communicate in a fail safe manner. The queue is a temporary storage location which messages can be sent and received reliably when destination is reachable. This enables communication across networks and between computers running Windows only which may not always be connected (versus sockets and other network protocols assure we that direct connections always exist). The Microsoft Distributed Transaction Coordinator(MS DTC)allows multiple operations on multiple queues to be wrapped in a single transaction.

### RabbitMQ vs. MSMQ
- **Centralized vs decentralized** message broker : messages are stored on a central server or cluster of servers, client sends messages to that central server, and then a subscriber can then retrieve that message (VS MSMQ is decentralized : each machine has its own queue)
- **Multi-platform messaging broker** versus **Windows only**: integration with these different platforms
- **Standards** versus **no standards**: AMQP versus own proprietary messaging format

### RabbitMQ management plugin
It provides a browser-based user interface to administer the message broker, as well as a HTTP-based API
for the management and monitoring of the RabbitMQ server: 
- declare, list, and delete exchanges, queues, bindings, users, virtual hosting permissions, 
- monitoring queue length, message rates, and data rates per connection
- sending and receiving messages
- monitoring the Erlang processes, file descriptors, and memory use, 
- force closing connections and purging queues.


> Go to http://localhost:15672  and then guest/guest
> go to exchange create a TestExchange, go to Queue and create Testqueue
> back to exchange and publish "hello in TestExchange" and back to queue and select Testqueue and then get message.
> if we don't want the messages to be requeued then in Testqueue => Ack Mode: Ack message requeue false { Nack message requeue true, Ack > message requeue false, Reject requeue true,Reject requeue false}


### AMQP Messaging Standard
**RabbitMQ** is built on top of the AMQP protocol; a network protocol that enables client's applications to communicate with the compatible messaging system.

### How's that work?
a message protocol works by receiving messages from a client or publisher and broker routes a message to a receiving application or consumer via an exchange, which acts as a mailbox, it sends a message to a queue by using different rules called bindings (direct routing, fanout, topic, headers) all within the message broker which delivers the message from the queue to a consumer.
The consumer (subscriber to the queue) pulls out the queue when a message is published, a publisher can specify various different messaging attributes which will be used by the message broker.
![pic](src/RabbitMq/Examples/images/figure1.JPG)
#### Message acknowledgements
The AMQP protocol has a mechanism for message acknowledgements (ACK) to deal with network unreliability and app failures; when a message is delivered to a consuming application, the consumer notifies the broker, either automatically, or as soon as the app developer decide so. 
When message ACK are used, the message broker will only remove the message from the queue when it receives a notification for that message. If a user messages are routed by the routing key (acts like a filter), it cannot be routed anywhere, it can either be returned to the sender, dropped, or if configured, be placed on a dead letter queue which is monitored.

**Exchanges** :
- They are AMQP entities where messages are sent to the message broker. 

- They take a message and then route it to one or more queues. 

- The type of routing depends on the exchange type used in different exchange rules (bindings).

**Types of exchanges**:

- **Direct exchanges**: queue binds to the exchange using a routing key, ideal for publishing a message onto just one queue  (message and queue keys must match)
![pic](src/RabbitMq/Examples/images/figure2.JPG)
 e.g. used to distribute messages between multiple work processes in a round robin manner

- **Fanout exchanges**: routes messages to all queues that are bound to it (routing key is ignored = broadcast), ideal for the broadcast
![pic](src/RabbitMq/Examples/images/figure3.JPG)
 e.g sync online game scores, weather updates, chat sessions between groups of people

- **Topic exchanges**: one or many queues based on pattern matches between the message routing key
![pic](src/RabbitMq/Examples/images/figure4.JPG)
e.g. multi-card/wild carded routing key  of messages to different queues. If * hash are used in binding then topic exchanges = fanout exchanges, if not used then topic exchanges = direct exchanges  

- **Header exchanges**: routing of multiple attributes that are expressed in headers (i.e. routing key/queue is ignored = only express one piece of information)
![pic](src/RabbitMq/Examples/images/figure5.JPG)
**Header exchanges** looks like a supercharged direct exchanges, as the routing is based on header values (also used as direct exchanges when routing key is not string)

Each exchange is declared with a set of attributes :
- **name**: name of the exchange, 
- **durability** flag: whether or not the messages sent to the exchange survive a broken server or restart by persisting the messages to disk.
- **auto-delete** flag: if the exchange is deleted when all the other queues are finished using it
- **arguments**: arguments that message broker dependent on.

> The AMQP message brokers contain a default exchange (pre-declared) that is a direct exchange with no name (empty string); useful for simple app where the queue that is created it is bound to it with a routing key, which is the same as the queue name. 
> e.g declaring a queue with the name 'payment requests', the message broker will bind it to the default exchange by using the 'payment request' as the routing key. i.e. the default exchange makes it looks as it directly delivers messages to queues (not technically happening). 


**Queues, Bindings, and Consumers**
**first-in first-out** basis, it must be declared. If the queue doesn't already exist, it will be created. If the queue already exists, then re-declaring the queue will have no additional effect on the queue that already exists

**Queues have additional properties over exchanges**

- **Name** : name of the queue (255 char max), can be picked by the app, or it can be automatically named by the broker that generates it. 

- **Durable**, whether the queue and messages will survive a broker or a server re-start,queue is persisted to disk. This makes only the queue persistent, and not the messages, durability means the queue will be re-declared once the broker is re-started. If we want the messages to be also persisted, then we have to post persistent messages. Making queues durable does come with additional overhead => decide if the app can't lose messages or not.

- **Exclusive**: is used by only one connection, and the queue will be deleted when that connection closes.

- **Auto Delete**: is deleted when a consumer or subscriber unsubscribes. 

**bindings** are defined when we need to define rules that specify how messages are routed from exchanges to queues, they may have an optional routing key attribute that is used by some exchange types to route messages from the exchange to the queue.

![pic](src/RabbitMq/Examples/images/figure6.JPG)

If an **AMQP message** cannot be routed to any queue (e.g. missing valid binding from the exchange to that queue) then it either dropped, or returned to the publisher, depending on the message attributes the publisher has set.


**From systems that consume messages perspective**, storing messages in queues is good, provided that there are apps on the other side of the queues to consume those messages.

**Consumers/subscribers with a set of queues**
Let assume an apps will register as **consumers/subscribers** to a **set of queues**, a common  scenario will be to balance a load of apps feeding from the queues in a high volume scenario. When a consuming application acts on a message from the queue, it is possible that a problem could occur and lead into a message lose, further, when an app acts on a message, that message is removed from the queue, but we need to make sure that the message has been successfully processed before that to happen. 

![pic](src/RabbitMq/Examples/images/figure7.JPG)


The **AMQP protocol** gives a set of options to **remedy** that situations (i.e. when a message is removed from the queue):
- The **message is removed** once a **broker** has sent the **message** to the **application**.
- Or, the **message is removed** once the **application** is sent an **acknowledgement message** back to the **broker**.

![pic](src/RabbitMq/Examples/images/figure8.JPG)

With an **explicit acknowledgement**, it is up to the **app** to decide when to **remove the message** from that **queue** (received a message, or finished processing it. 

![pic](src/RabbitMq/Examples/images/figure9.JPG)

If the consuming **app crashes before** the **acknowledgement** has been sent, then a **message broker** will try to **redeliver** the message to another consumer. When an app **processes a message**, that processing may or may not succeed. If the processing fails for any reason(e.g. database time outs), then a consumer app can reject the message. The app then can ask the broker to discard the message or re-queue it. 

> If there's only one consumer app subscribed to the queue, we need to **make sure** that we **don't create an infinite message delivery loop** by rejecting and re-queuing the message from the same consumer.

### RabbitMQ Client Library
We need to install the RabbitMQ client library for dot net. to develop software against RabbitMQ. [API guide to client library API](https://www.rabbitmq.com/devtools.html)

```sh
Install Package RabbitMQ.Client
```

RabbitMQ client library is an implementation of the **AMQP client library for C#**. The client library implements the **AMQP specifications**. The API is closely modeled on the **AMQP protocol specification**, with little additional abstraction. 

The core API interface and classes are defined in the **RabbitMQ.Client namespace**. The main **API interface** and classes are:

- **IModel**: represents AMQP data channel, and provides most of the AMQP operations. 

- **IConnection**: AMQP connection to the message broker

- **ConnectionFactory** : constructs *IConnection* instances. 

*Other useful classes include*: 

- **ConnectionParameters**: used to configure the connection factory

- **QueuingBasicConsumer**: receives messages delivered from the server.

```sh
//Connecting to a message Broker
ConnectionFactory factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
IConnection connection = factory.CreateConnection;
IModel channel = connection.CreateModel;
```

```sh
//Exchanges and queues are Idempotent
//Idempotent operation : if the exchange/Queue is already there it won't be created, otherwise it will get created.
var ExchangeName = channel.ExchangeDeclare ("MyExchange", "direct");
channel.QueueDeclare("MyQueue");
channel.QueueBind("MyQueue", ExchangeName ,"");
```

### Example of a Standard Queue 
using client API we have one producer posting the payment message onto a "StandardQueue" queue, and one consumer reading that message from the "StandardQueue" queue. It looks like the producer posts directly onto the "StandardQueue" queue, instead of using an exchange. 

![pic](src/RabbitMq/Examples/images/figure10.JPG)

What happens is under the covers we are posting to the **default exchange**; **RabbitMQ broker** will bind "StandardQueue" queue to the default exchange using "StandardQueue" (name of the queue) as the rooting key. Therefore a message publishes to the default exchange with the routing key "StandardQueue" will be routing to "StandardQueue" queue.

**Declaring** the **queue** in **RabbitMQ** is an **idempotent operation**, i.e. it will only be created if it doesn't already exist. 
> Generally speaking, an item hosting operation is one that has no additional effect if it is called more than once, with the same input parameters. 


```sh
string QueueName = "StandardQueue";

_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest"};
_connection = _factory.CreateConnection();
_channel = _connection.CreateModel(); 

//tells the broker the queue is durable. i.e. that queue is persisted to disk and will survive,
//or be re-created when the server is restarted.
//Exchanges and queues are Idempotent
//Idempotent operation : if the exchange/Queue is already there it won't be created, otherwise it will get created.                     
 _channel.QueueDeclare(queue:QueueName, durable:true, exclusive:false, autoDelete:false, arguments:null);         

```

```sh
//Send message  
//payment.Serialize(): converts payment message instances into a compressed bytes[] to a json representation            
 channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: payment.Serialize());
```

```sh
//receive message              
 _channel.BasicConsume(queue: QueueName, noAck: true, consumer);

//DeSerialize is user-defined extension method 
 var message = (Payment)consumer.Queue.Dequeue().Body.DeSerialize(typeof(Payment));
```
> Consumers last so long as the channel they were declared on, or until the client cancels them.

[more ...](src/RabbitMq/Examples/StandardQueue/Program.cs)


### Example of a Multiple Queues (i.e. Worker Queue or multiple consumers) 
The idea is that messages from the queue are shared between one or more consumers, it commonly used when we want to share the load, between consumers when processing higher volumes of messages.
![pic](src/RabbitMq/Examples/images/figure11.JPG)


**[Producer](src/RabbitMq/Examples/WorkerQueue_Producer/Program.cs)**
```sh
string QueueName = "WorkerQueue_Queue";

_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest"};
_connection = _factory.CreateConnection();
_channel = _connection.CreateModel(); 

//tells the broker the queue is durable. i.e. that queue is persisted to disk and will survive,
//or be re-created when the server is restarted.                     
 _channel.QueueDeclare(queue:QueueName, durable:true, exclusive:false, autoDelete:false, arguments:null);        

```

```sh
//Send message  
//payment.Serialize(): converts payment message instances into a compressed bytes[] to a json representation            
 channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: payment.Serialize());
```

 **[Consumer](src/RabbitMq/Examples/WorkerQueue_Consumer/Program.cs)**
```sh
//(Spec method) Configures Quality Of Service parameters of the Basic content-class.
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new QueueingBasicConsumer(channel);
//we want to expect an acknowledgement message (noAck: false)
channel.BasicConsume(queue: QueueName, noAck: false, consumer: consumer);

```

```sh
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
}

```
> prefetchCount: 1 (load balancing among workers) means that RabbitMQ won't dispatch a new message to a consumer, until that consumer is finished processing and acknowledged the message, if a worker is busy (noAck) RabbitMQ will dispatch a message on the next worker that is not busy.





### Publish and Subscribe queues
The messages are sent from the exchange to all consumers that are **bound to the exchange**. i.e. the messages are not picked up by multiple consumers to distribute load, but instead all subscribed consumers with interest in receiving the messages. Unlike the previous example where we defined the **queue directly** which's use a **default exchange** (with routingKey = queue_name) behind the scenes, here we will set up an explicit **Fanout** exchange. A **fanout exchange** routes messages to all of the queues that are bound to it(i.e. routing key is ignored). 

![pic](src/RabbitMq/Examples/images/figure12.JPG)

**[Publisher](src/RabbitMq/Examples/PublishSubscribe_Publisher/Program.cs)**
> If queues are bound to a fanout exchange, when a message is published onto that exchange a copy of that message is delivered to all those queues.

```sh
string ExchangeName = "PublishSubscribe_Exchange";
_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
_connection = _factory.CreateConnection();
_channel = _connection.CreateModel();

//Idempotent operation : if the exchange/Queue is already there it won't be created, otherwise it will get created.
_channel.ExchangeDeclare(exchange: ExchangeName, type: "fanout", durable: false);

//We are publishing directly to an and exchange any queues that have been bound to that exchange will receive the message
//No need for routingKey Vs Default exchange (which bears the name of the queue!) 
_channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: message.Serialize());
```
**[Subscriber](src/RabbitMq/Examples/PublishSubscribe_Subscriber/Program.cs)**
```sh
string ExchangeName = "PublishSubscribe_Exchange";
_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
using (_connection = _factory.CreateConnection())
{
    using (var channel = _connection.CreateModel())
  {
	//Idempotent operation : if the exchange/Queue is already there it won't be created, otherwise it will get created.
	channel.ExchangeDeclare(exchange: ExchangeName, type: "fanout");

	//this uses a system generated queue name such as amq.gen-qVC1KT9w-plxzpV9MVId9w
    var queueName = channel.QueueDeclare().QueueName;
	
    channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "");
	
    _consumer = new QueueingBasicConsumer(channel);
	
	//consumer has created its own queue, and subscribed itself to the exchange
	//It will receive all messages that are sent to the exchange ("PublishSubscribe_Exchange")
	//noAck: true =>  No waiting for a message acknowledgement before receiving the next message.
	//We don't need to as our subscriber application is reading from its own queue 
	//it takes msg as it can deal with, no work split no load balancing each subscriber will receive the same msg copies
     channel.BasicConsume(queue: queueName, noAck: true, consumer: _consumer);
    
	while (true)
    {
        var ea = _consumer.Queue.Dequeue();
        var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
		
		//no need to send message acknowledgement to tell RabbitMQ that we're finished with a message, 
		//because we want all messages to be sent to every consumer, otherwise get removed from the queue
		//channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
  }
}
```

### Direct routing
The **routing key** will be used, so direct messages to a specific consumer (Vs **fanout exchange** where routing key would be ignored). 
In this example, the producer app will post two different types of messages, **card payment** and **purchase order** messages posted to the exchange, each using specific **routing key**. We create two different **consuming apps** one looking out for **card payments**, and the other is only interested in **purchase orders**. They pick up their messages based on that **routing key**.

![pic](src/RabbitMq/Examples/images/figure13.JPG)

**[Publisher](src/RabbitMq/Examples/DirectRouting_Publisher/Program.cs)**
```sh
string ExchangeName = "DirectRouting_Exchange";
string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";
string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";

_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
_connection = _factory.CreateConnection();
_channel = _connection.CreateModel();

 //type: direct exchange
 //Queues an exchanges are idempotent
_channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");

//durable: true=> queues are persisted to disk, if the server ever crashes or resets,
//the queue will be persisted and come back to life.
_channel.QueueDeclare(CardPaymentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
_channel.QueueDeclare(PurchaseOrderQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

//Binding: exchange name, the queue name and the routing key
//routingKey: determines what queue the message is routed to.
_channel.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName, routingKey: "CardPayment");
_channel.QueueBind(queue: PurchaseOrderQueueName, exchange: ExchangeName, routingKey: "PurchaseOrder");

_channel.BasicPublish(exchange: ExchangeName, routingKey: routingKey, basicProperties: null, body: message);

```

**[CardPayment Subscriber](src/RabbitMq/Examples/DirectRouting_Subscriber1/Program.cs)**

```sh
string ExchangeName = "DirectRouting_Exchange";
string CardPaymentQueueName = "CardPaymentDirectRouting_Queue";

_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
using (_connection = _factory.CreateConnection())
{
    using (var channel = _connection.CreateModel())
    {
        //Queue binding to exchange and listen to CardPayment messages
		//Queues an exchanges are idempotent
		channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
        channel.QueueDeclare(queue: CardPaymentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: CardPaymentQueueName, exchange: ExchangeName, routingKey: "CardPayment");
		
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
		//in case of the consumer succeeds Ack is sent back to the broker, message (successfully processed) is discarded 
		//from the queue and worker is ready to process another one.
        channel.BasicConsume(queue: CardPaymentQueueName, noAck: false, consumer: consumer);

        while (true)
        {
			var ea = consumer.Queue.Dequeue();
			var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
			var routingKey = ea.RoutingKey;
			
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}


```


**[PurchaseOrder Subscriber](src/RabbitMq/Examples/DirectRouting_Subscriber2/Program.cs)**

```sh
string ExchangeName = "DirectRouting_Exchange";
string PurchaseOrderQueueName = "PurchaseOrderDirectRouting_Queue";
_factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
using (_connection = _factory.CreateConnection())
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
        }
}
```

### Message queuing

- They are **components** used for **inter-process communication** or for **inter-thread communication** within the same process. They use a queue for messaging, which is passing data between systems. 

- They provides an **asynchronous communications protocol**. The sender and the receiver of the message do not need to interact with the message queue at the same time. Messages placed onto the queue are stored until the recipient retrieves them.

Next, a few considerations that can have substantial effects on **transactional semantics**, **system reliability**, and **system efficiency**. 

- **Durability**: messages may be kept in memory, written to disk, or even committed to a database if the need for reliability indicates a more resource-intensive solution. 

- **Security policies**: we define which application should have access to the same messages. 

- **Message purging policies**: where queues or messages may have a time-to-live, which defines when they will be automatically deleted. 

- **Message filtering**: where some systems support filtering data so a subscriber may only see messages matching some pre-specified criteria of interest. 
- **Delivery policies**: where we define the need to guarantee that a message is delivered at least once or no more than once. 

- **Routing policies**: where in a system with many queue servers, what server should receive a message or a queue's messages. 

- **Batching policies**: this is where we define if messages should be delivered immediately or should the system wait a bit and then try to deliver many messages at once. 

- **Queueing criteria**: determines when should a message be considered unqueued, when one queue has it or when it's been forwarded to at least one remote queue or to all queues 
- **Notification**: when a publisher may need to know when some or all of the subscribers have received a message. 

### Uses for Message Queueing

- **Decoupling**: introducing a layer between processes, message queues create an implicit interface that both processes implement. This allows we to extend and modify these processes independently by simply ensuring that they adhere to the same interface requirements. 

- **Redundancy**: Sometimes processes fail when processing data. Unless that data is persisted, it's lost forever. Queues mitigate this by persisting data until it has been fully processed. The put, get, delete paradigm, which many message queues use, requires a process to explicitly indicate that it is finished processing a message before that message is removed from the queue, ensuring that data is kept
safe until we are done with it (i.e. message acknowledgements).


- **Scalability**: message queues decouple the processes, it's easy to scale up the rate which messages are added to the queue or processed simply by adding another process. No code needs to be changed, no configurations need to be tweaked. Scaling up is as simple as adding more processes to the backend solution. 

- **Resiliency**: when part of the architecture fails, it doesn't need to take the entire system down with it. Message queues decouple processes. So if an application that is processing messages from the queue fails, messages can still be added to the queue to be processed when the system recovers. This ability to accept requests that will be retired or processed at a later date is often the difference between an inconvenienced customer and a frustrated customer. 

- **Delivery guarantees**: The redundancy provided by message queues guarantees a message will be processed eventually so long as a process is reading the queue. No matter how many processes are pulling data from the queue, each message will only be processed a single time. This is made possible because retrieving a message reserves that message, temporarily removing it from the queue till it has been acknowledged. Unless the client specifically states that it is finished with that message, the message will be placed back onto the queue to be processed after a configurable amount of time. 

- **Ordering guarantees**: the order of which a data is processed is important. Message queues are inherently ordered and capable of providing guarantees that the data will be processed in a specific order. Message queuing such as RabbitMQ guarantee that messages will be processed using a first-in, first-out order. So the order in which messages are placed onto a queue is the order in which they'll be retrieved from it.

- **Buffering**: In any system, there might be components that require different processing times, e.g. it might take less time to upload an image than it does to apply a set of filters to it. Message queues help the set tasks operate at peak efficiency by offering a buffering layer. The process writing to the queue can write as fast as it is able to instead of being constrained by the readiness of the process reading from the queue. This buffering helps control and optimize the speed in which data flows through the system. 

- **Asynchronous communication**: sometimes, we don't want only to process a message immediately. Message queues enable asynchronous processing, which allows us to put a message onto the queue without processing it immediately, queue up as many messages as we like and then process them at the leisure. 

### System resilence
The system should be able to cope with change as well as minor or major disruptions.

> The power or ability to return to the original form or position after being bend, compressed or stretched => cope with problems and not be hardened against failure (elasticity).

> The capacity to recover quickly from difficulties => fast recovery of systems more explicitly.

> The ability of a system to cope with change=>comes from supply chain background and is more about keeping a system running.


### Asynchronous Services
a *synchronous communication*, a call is made to a remote server. We send blocks until the operation completes (easy to reason about),while **asynchronous communication**, the caller doesn't wait for the operation to complete before returning. It may not even care whether or not the operation completes at all.
 
These two different **modes of communication** that can enable two different **styles of collaboration**:
- **request and response**: a client initiates a request and waits for the response, This model clearly aligns well to synchronous communication, but can work for asynchronous communication as well. We might kick off an operation and register a callback asking the server to let us know when my operation is completed.

- **Event-based** collaboration where we **invert flow**, instead of the client initiating requests asking for things to be done, it instead says that something has happened and expects other parties to know what to do. 
	- Event-based systems by their nature are asynchronous. This means that the processing of these messages doesn't need to be centralized in any one place. Instead, we can have many consumers to process messages. e.g. we have multiple consumers working on the messages.
	- Event-based collaboration is also highly decoupled. The client that emits an event doesn't have any way of knowing who or what will react to it, which also means that you can add new subscribers to the event without the client ever needing to know. 
	
### [Setting up the RabbitMQ Management Portal](https://www.rabbitmq.com/management.html)

```sh
#open cmd or powershell
rabbitmq-plugins enable rabbitmq_management
```
### [Topic Based Publisher and Subscribe](src/RabbitMq/FinSoft)
The **direct** message **exchange** type use routing key to route messages to different consumers or subscribers, The **topic exchange** is similar and more powerful to use. Messages sent on topic exchange don't just have an arbitrary routing key as before. The **routing key** must be a list of words separated by **"."** and eventually a wild card **('star')**, the words (limit of **255 bytes**) can be anything that specifies some features connected to the message.

![pic](src/RabbitMq/Examples/images/figure14.JPG)

A message sent by a particular **routing key** will be delivered to all the **queues** that are **bound** with a **matching key**; Two important **special cases** for **binding keys** :
- **'star'** can be substituted for exactly one word, 
- **'hash'** can be substituted for zero or more words.

```
payment.* : consumer is interested in any message that starts with payment.
```
### Remote Procedure Calls
An example is when we post the message onto a queue, a consumer act on the message, and then a reply is posted back via the queue to the original producer application.

**[The sample project](src/RabbitMq/FinSoft)** is split into: 
- A **[client](src/RabbitMq/FinSoft/RestApi/Controllers/DirectCardPaymentController.cs)**, which is our web API : The client application posts messages directly onto a queue. For each message that gets posted, the application waits for a reply from a reply queue. This essentially makes this a synchronous process.

- **[server](src/RabbitMq/FinSoft/DirectPaymentCardConsumer/RabbitMQ/RabbitMQConsumer.cs)** which is the consumer: When a message is posted to the server from the client, a correlation ID is generated and
attached to the message properties. The same correlation ID is put onto the properties in a reply message. This allows us to easily tie together the replies in the originating messages if you store them for retrieval later. 

![pic](src/RabbitMq/Examples/images/figure15.JPG)

> The client posts a message to the RPC queue that has a correlation ID of 12345. This message is received by the server and a reply is sent back to the client on a reply queue with the same correlation ID of 12345.

# II) Microservices and NService Bus

## Monolith 

application where everything is contained in a single program using one technology.

**Benefit of a monolith**
- **Ease of deployment**

- **Most programmed applications** in the world, and tend to be simple in architecture (Junior developers kick in)

- **No external dependencies**

- Easy to **test** as a whole, and setting up multiple environments should be fairly straightforward

- **Project** is easily **shared** among **developers** through a source control.

**Monolithic problems**

- Components are called **synchronously**, i.e. longtime before the user sees some result

- Tight **horizontal coupling** going on between the components in a synchronous way

- It's common to use one **same database** for each of the components which could lead to potential **integration through database**.

- How to manage **roll back** : *user retry*, or should the *system retry*?

- The **risk of overloading the server** is substantial (chaining problem)

- Difficult to have **multiple teams** doing **each component separately** due **tight coupling** between components (a component changes, the whole application has to be retested)

- Over time, **features** are **added** to the **solution** until the **architecture** can't support all the features anymore, then a **rewrite** is required. 

- When **application** grows it becomes **complex**, **brittle** and **hard to maintain**.

- Most likely **performance** becomes an issue, because **components** are called **synchronously** after each other’s.

- Monolith is limited to one **technology stack**.


## Distributed Applications
applications or software that runs on multiple computers within a network at the same time and can be stored on servers or with cloud computing

**Important concepts** to keep in mind :

- **High cohesion**: the pieces of functionality that relate to each other should stick together in a service and unrelated pieces should be pushed outside.

- **Loose coupling**:in order to talk to the other services we must hide as much complexity as possible within the service and expose as less as possible through an interface (or **Contract**), e.g. ability to change some logic within one service without disrupting other services, avoiding to **redeploy** them as well.

>> Domain Driven Design help us to find boundaries through the Bounded contexts.


**Coupling** is the way different services depend on each other:

- **Platform** coupling: a service can only be called by an app built with the same technology, e.g. ties all current and future  development within an organization to one particular platform.

- **Behavioral** coupling: a caller has to know exactly the **method name** (+ any possible **parameters**) it's calling, e.g. change the way the service is called and all surrounding services have to be **re-adjusted and redeployed**.

- **Temporal** coupling: refers to an SOA app as a whole can't function when one (or several) service is down. e.g. calls are handled synchronously and server is waiting for response and services architecture rely on each other services to be up & running, it brings the following issues :

	- How is the user notified of the error
	- how do we roll back the other services
	- how do we handle retries

>> Note that this has nothing to do with synchronous or asynchronous code, like the async and wait syntax in C#, using that pattern, resources for the web server for instance are made available for other requests while servers do their work, but the user still has to wait for them all to complete!


The **Eight Fallacies of Distributed Computing** by [Peter Deutsch](http://www.codersatwork.com/l-peter-deutsch.html):

*1.	The network is reliable*

*2.	Latency is zero*

*3.	Bandwidth is infinite*

*4.	The network is secure*

*5.	Topology doesn't change*

*6.	There is one administrator*

*7.	Transport cost is zero*

*8.	The network is homogeneous*


### Distributed Architecture:

- **Service-Oriented Architecture** (SOA) : where many of the components in an application are services - implementation could be a web application that calls a web service, that calls another web service, ...

- **Microservices** (aka SOA 2.0 or SOA done properly): complex applications where the services are small, dedicated and autonomous to do
a single task or features of the business, they neither share implementation code, nor data. Every microservice has its own database or  data store suitable to a particular kind of service. They communicate using language-agnostic APIs, The services are loosely coupled and don't have to use the same language or platform.


![pic](https://martinfowler.com/bliki/images/microservice-verdict/productivity.png)

**Properties of microservices**: 

- **Maintainable**: easier to maintain, very loosely coupled and have a high cohesion, team work separately, the overall architecture is more complex, but an individual microservices architecture is probably not complex. 

- **Versioning**:  new version of the service could run side-by-side with an old version, it is also possible to let one microservice has multiple messaging process endpoints, each supporting different versions. 

- **Each their own**: technology best suited for the service, e.g. language, framework, platform, and database suited to the service.

- **Hosting**: flexible. Physical machines or virtual machines in the cloud or docker container that could be scaled out.

- **Failure Isolation**: app as a whole keeps functioning, the message sticks in the queue, and can be picked up as soon as the service is running again. 

- **Observable**: each service on a separate VM or Docker container are highly observable, because CPU and memory can be monitored for each service individually, and will become immediately apparent what service should be fixed or optimized.

- **UI**: each autonomous service should have and expose its own user interface. That way from a UI perspective, when one service fails, all UI elements are still shown, except for the UI from that one service (SPA).

- **Discovery**:  services to discover each other in the form of name IP address resolution, for example, it prevents a tight coupling between the service and its URI (product like Consul or ZooKeeper). 

- **Security**: security mechanism such as OAuth 2 and OpenID Connect. 

- **Deployment**: a team can work on one microservice, and also deploy it separately, Continuous deployment, essentially deployment after each check in is very straight forward to implement.

>> Microservice architecture solves a lot of problems, but it also introduces complexity, security, UI, discoverability, hosting, and messaging infrastructure, and monitoring, all become much more complex than programming a monolithic application.


### Distributed Architecture technology:

1. **RPC** (Remote Procedure Call) : is a way to call a class' method over the wire, different programming platforms each develop their own way RPC (.NET Remoting and Java RMI and later WCF based on more standardized SOAP or Simple Object Access Protocol using WSDL).
	- High degree of **behavioral** (proxy classes) and **temporal coupling** 
	- Although ** WSDL** allows **methods discovery**  so many programming frameworks and languages can consume the servers, it tends to be implemented slightly different by the different platforms, so there is still **slight platform coupling**.
	
2. **[REST](https://martinfowler.com/articles/richardsonMaturityModel.html)** (Representational State Transfer): is using the semantics of the transport protocol, commonly used protocol is HTTP. One of the properties is that the methods in the service are not directly exposed, all resources like data are available as specific URIs, and want to do with it is partly determined by how the call to the URI is made (HTTP verb such as Get, Post, Put, Delete).


![pic](https://martinfowler.com/articles/images/richardsonMaturityModel/overview.png)

>> Swamp of POX, where POX stands for Plain Old XML which refers to RPC with SOAP, where RPC is mostly ignoring the underlying protocol such TCP.

>> Hypermedia controls is a way to get the URIs from the service, and a consumer knows where a certain resource is located, e.g. in the REST model, when creating data with a Post call, the response returns the unique URL where the new resource is located.

**Rest and Coupling**

- Lower platform coupling 

- Behavioral coupling is still present but can get very low (Uri's or resource location).

- Temporal coupling because REST services still have to be up to do their jobs, and consumers still have to wait for the response.


3. **Asynchronous Messaging**: using a service bus (set of classes around the sending and receiving of messages) enables different services to send and receive messages in a loosely-coupled way, every service has an endpoint with which can receive and send messages. The messaging system should be **dumb pipes** and contain no business logic (Vs **ESB** such as MS BizTalk), every domain logic should be in the service itself. The messaging system only routes the message to the inbox of another service, or multiple services in the case of a published event, If the receiving service is up, it will be notified via the service bus that a new message has arrived, and if processed successfully, it will be deleted from the queue, after which the service can process the next one. Each endpoint is connected to one particular queue, but one service can contain multiple endpoints.

**Microservice and Coupling**

- *Loose platform coupling*: with the exception the inability for some platform to connect to the message queue.

- *Loose Behavioral coupling*: Only message with some data *VS* RPC and REST where the behavior is dictated by the caller which should have some knowledge about the request that handled by the receiver. Whereas in Microservices, how the request is handled is entirely determined by the service that has to process it.

- *Loose Temporal coupling*: When sending a command or receiving an event, the service doesn't have to be up, because the message is safe in a queue until the service becomes available.


>> Fallacies of distributed computing still apply: network dependency, latency or security, ...

>> Asynchronous Messaging system uses eventual consistency which need to be managed efficiently. 


### [NServiceBus](https://particular.net/nservicebus)

It's a .NET Framework that enables us to implement communication between apps using messaging in microservice style architecture. It's part of a suite called the Particular Service Platform. The framework lies on top of messaging backends or transports, i.e. an abstraction of the messaging backends. NServiceBus started out as a framework supporting only MSMQ, but nowadays is also supporting transports like RabbitMQ and Azure and even SQL Server, the transport is a simple config detail.

NServiceBus is very pluggable and extensible, It comes in different NuGet packages supporting dependency injection frameworks and databases... 

>> The example here are based on *"eCommerce"*  [project](src/eCommerce)

The core of NServiceBus consists of: 

- **NServiceBus** - Required: contains the complete framework (NuGet packages) that support transports such as MSMQ, RabbitMQ, Azure, SQL Server which come in separate NuGet packages each.

- **NServiceBus.Host** (Deprecated) - Optional : self-host different app styles such as ASP. NET MVC and WPF. Self-hosting is optional, but we can create a DLL containing this service, and let NServiceBus do the Hosting. This package contains an executable that behaves as a command line application for debugging, and it can be easily installed as a Windows service. 

- **NServiceBus.Testing**: help with unit testing specially with **sagas**.



#### Messages: Commands and Events

- **Commands** :  are messages or C# class containing data in the form of properties, they can have multiple senders, but always have one receiver, either they should be derived from ICommand interface,or implemeted using an Unobtrusive mode. 
As a convention, commands Names in the imperative e.g. *ProcessOrder* or *CreateNewUser*

```sh
 await endpoint.Send(message: new ProcessOrderCommand
            {
                OrderId = Guid.NewGuid(),
                AddressFrom = order.AddressFrom,
                AddressTo = order.AddressTo,
                Price = order.Price,
                Weight = order.Weight
            }).ConfigureAwait(continueOnCapturedContext: false);
```


- **Events** : are implemented as C# interfaces. They are different because they always have one sender and multiple receivers (i.e. commands opposite). Events implement the publish/subscribe pattern, so receivers interested in a specific event must register themselves with the sender. NServiceBus stores the subscriptions in the configured storage and we must mark all event classes with the IEvent marker interface. Events are in the past tense like OrderProcessedEvent or NewUserCreatedEvent. 

> Under the cover, NServiceBus will create the implementing class, we provide the content of the message by specifying the Lambda. 

```sh
...
public async Task Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        {
           ...
            await context.Publish<IOrderProcessedEvent>(messageConstructor: e =>
             {
                 e.AddressFrom = message.AddressFrom;
                 e.AddressTo = message.AddressTo;
                 e.Price = message.Price;
                 e.Weight = message.Weight;
             });            
        }

```

> NServiceBus uses a built-in IoC container, which is a lean version of **Autofac** contained in the core, IoC inject NServiceBus-related types into objects managed by NServiceBus, such as a class implementing IHandleMessages, ICommand, IEvent,... etc, 

>At Endpoint startup NServiceBus does the Assembly Scanning to find all types that need such as ICommand, IEvent (or Unobtrusive mode) and IHandleMessages, and registering all the types it needs. 

> If we want to inject IBus instances into MVC controllers, etc., we can plug in virtually any existing container...

> If we don't want to scan all assemblies, the scanning can be limited in the config file to scan only certain assemblies.


#### Routing Messages

- To send command to the same endpoints we use a routing option config instead of specific endpoint name as a string in the send method.

- For events we have to specify where the endpoint should register the subscription. 

There are 2 choices for routing : 
- config file: xml file config

```sh
<configuration>
  <configSections>
    <section name="MessageForwardingInCaseOfFaultConfig" type="NServiceBus.Config.MessageForwardingInCaseOfFaultConfig, NServiceBus.Core"/>
    <section name="UnicastBusConfig" type="NServiceBus.Config.UnicastBusConfig, NServiceBus.Core"/>
    <section name="AuditConfig" type="NServiceBus.Config.AuditConfig, NServiceBus.Core"/>
  </configSections> 
  <MessageForwardingInCaseOfFaultConfig ErrorQueue="error"/>
  <UnicastBusConfig>
    <MessageEndpointMappings>
	 <!-- Option 1 -->
     <!--<add Assembly="eCommerce.Messages" Endpoint="eCommerce.Order"/>
	 <!--Option 2-->
     <!--<add Assembly="eCommerce.Messages" Namespace="eCommerce.Messages" Endpoint="eCommerce.Order"/>-->
	 <!--Option 3-->	 
     <add Assembly="eCommerce.Messages" Type="eCommerce.Messages.ProcessOrderCommand" Endpoint="eCommerce.Order"/>
    </MessageEndpointMappings>
  </UnicastBusConfig>
  <AuditConfig QueueName="audit"/>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
  </startup>
</configuration>
```

- routing API : NServiceBus is moving more to that option

**Commands example**

```sh
var transport = endpointConfiguration.UseTransport<MyTransport>();
var routing = transport.Routing();

//Option 1 : route all messages in the assembly were the ProcessOrderCommand 'classes' in are routed to the "eCommerce.Order" endpoint
//XML :  <add Assembly="eCommerce.Messages" Endpoint="eCommerce.Order"/>
routing.RouteToEndpoint(
    assembly: typeof(eCommerce.Messages.ProcessOrderCommand).Assembly,
    destination: "eCommerce.Order");

//Option 2:  limit that to a certain namespace in that assembly, e.g. "eCommerce.Messages"
//XML : <add Assembly="eCommerce.Messages" Namespace="eCommerce.Messages" Endpoint="eCommerce.Order"/> 
routing.RouteToEndpoint(
    assembly: typeof(ProcessOrderCommand).Assembly,
    @namespace: "eCommerce.Messages",
    destination: "eCommerce.Order");

//Option 3: limit routing for one messageType "ProcessOrderCommand"
//XML : <add Assembly="eCommerce.Messages" Type="eCommerce.Messages.ProcessOrderCommand" Endpoint="eCommerce.Order"/>
routing.RouteToEndpoint(
    messageType: typeof(eCommerce.Messages.ProcessOrderCommand),
    destination: "eCommerce.Order");

```

**Subscriber Event example**

Here the publisher endpoint is *"eCommerce.Order"*

```sh
var transport = endpointConfiguration.UseTransport<MyTransport>();

var routing = transport.Routing();

//Option 1
routing.RegisterPublisher(
    assembly: typeof(eCommerce.Messages.IOrderProcessedEvent).Assembly,
    publisherEndpoint: "eCommerce.Order");

//Option 2
routing.RegisterPublisher(
    assembly: typeof(eCommerce.Messages.IOrderProcessedEvent).Assembly,
    @namespace: "eCommerce.Messages",
    publisherEndpoint: "eCommerce.Order");

//Option 3
routing.RegisterPublisher(
    eventType: typeof(eCommerce.Messages.IOrderProcessedEvent),
    publisherEndpoint: "eCommerce.Order");

```

a **routing** for a **command** is obvious, i.e. we route the message to the **endpoint point** which should **receive** the **message**. 
For **events**, the routing must point to the **publisher** of the **event**, i.e. where the **subscription** should be **registered**. 

>> Note events are only interfaces that are shared between publisher and subscriber, they are not concrete classes.


#### Configuring NServiceBus

- The **configuration** of NServiceBus relies on **defaults**. 
- The **configuration** can be a **combination** of *code* and the *config file*
- **IConfigureThisEndpoint**: Classes implementing this interface are picked up when NServiceBus scans the assemblies, and they are called first when NServiceBus starts up.


```sh
public class EndpointConfig : IConfigureThisEndpoint
{
    public void Customize(EndpointConfiguration configuration)
    {
        // use 'configuration' object to configure scanning
		configuration.UsePersistence<InMemoryPersitance>();
		configuration.SendOnly();
    }
}
```

>> when endpoint is not specified, its name is taken from the namespace the configuration class resides in.

- **INeedInitialization** interface: assembly with one or more classes implementing this interface could define company defaults and conventions, and then reference the assembly from all projects that use NServiceBus. In that way, **IConfigureThisEndpoint** can be used to just override these if necessary. 

- **SendOnly** Endpoint: only send messages and not receive them. NServiceBus will not **waste processing and resources** on the **receiving** of **messages** if we create the bus as SendOnly.


### Message Serialization

NServiceBus has to serialize the message classes. The serialized classes form the body of the message in the underlying transport(XML/JSON).

```sh
configuration.UseSerialization<JsonSerializer>
```

>> Other serialization types supported out of the box are BSON and Binary, or you can write our own if needed.

### Logging

NServiceBus features a built-in logging mechanism, we can also install logging framework by downloading a supporting NuGet package for it. The default logging contains 5 logging levels.

NServiceBus-hosted mode, all logging messages are outputted to the console, they're also written to the trace object, which we configure the output using standard. NET techniques. 

The rolling file is also supported, and has a default maximum of 10 MB per file, and 10 physical log files. The default log level threshold for messages going to trace and the file is info, but it can be adjusted in the config file.

### Persistence Options

The **features of the transport** define what NServiceBus should store: 

- *Write you own* : Not a good idea

- **MSMQ** :  doesn't support subscriptions, so the handling of that must be done separately. 

- **State of sagas** has to be stored

- **No default** persistence: need to be defined otherwise NServiceBus will throw an exception at startup. 

- use **InMemoryPersistence** (out of the box) as core assembly built-in by Particular Software, this more suitable for testing and demo purposes. 


- **NHibernate** Persistence (SQL service, Oracle): some persistent classes might require an additional NuGet package and additional config is needed such as connection string in the connection. 

- when **installers** are **enabled**, **NServiceBus** will automatically **create the schemas necessary** when they are not present (has permission to do so).

- **RavenDbPersistence** , **Azure Storage** & **Azure Service fabric** Persistence

- Multiple **persistence mechanisms** :  NServiceBus could use separate storages for subscriptions, sagas, and saga timeouts in the Outbox feature

```sh
endpointConfiguration.UsePersistence<NHibernatePersistence, StorageType.Outbox>();
endpointConfiguration.UsePersistence<NHibernatePersistence, StorageType.Sagas>();
endpointConfiguration.UsePersistence<InMemoryPersistence, StorageType.GatewayDeduplication>();
endpointConfiguration.UsePersistence<InMemoryPersistence, StorageType.Timeouts>();

// This one will override the above settings
endpointConfiguration.UsePersistence<RavenDBPersistence>();
```

###  Transports

- **MSMQ** (Windows server native - limited to Windows): works in a decentralized way, i.e. nothing between the service; every server has its own queues stored locally. When a message is sent, it is placed in an outgoing queue local for the server, then the MSMQ system is  delivering the message to the incoming queue of another server (**store and forward**), once a message is sent, it will arrive at the destination and when the sender of the message goes down right after sending the message, or the receiver is down, the message will stick the queue until it can be delivered. Events with publish subscribe are not supported in MSMQ, NServiceBus has its own mechanism which use its persistent setting, i.e. every time the message is sent, the persistent storage is checked if there are any subscribers. 

- **RabbitMQ** : cross-platform broker, supports **AMQP** (standard protocol for messaging).It is centralized (need to be **clustered in a production** due to **single point of failure for the app**), i.e. one server or cluster running RabbitMQ on which the messages and queues reside. When a message arrives at RabbitMQ, it is processed by **an exchange**, which will route the message to one or more queues. **RabbitMQ** has a much *better built-in config routing* (NServiceBus uses this routing mechanism).

- **SQL Server**:  Messages are placed in a table when sent, and the receiving side is polling the table to look for
new messages. When it processes one successfully, it just deletes the message from the table. The table can be on
the separate SQL instance or the same one the application uses. NServiceBus uses a back off strategy to do the
poling. When no messages are in the queue, it will wait longer and longer before trying again, up to a maximum of
a configurable amount of time. The default maximum is 1 second. 

- **Microsoft Azure**:  NServiceBus can support it and use its internal routing :
	- **Azure Storage Queues**: simple storage mechanism (low cost) that supports the queuing and dequeuing of messages. . 
	- **Azure Service Bus** : more advanced and costly but enables bigger messages and lower latency among more options on the message level. 
	
**NServiceBus** is an **abstraction** of the **underlying transport**, e.g. as long as the project is small, we start with SQL Server as Transport, and when more messages are flowing through the system, we can switch (which's a mere config detail) to more **robust** transport as **RabbitMQ**. 

> Each transports architecture tends to be different, we need to choose the more suitable to the business operation.

### [Installers](https://docs.particular.net/nservicebus/operations/installers)

They are built into NServiceBus, e.g. create the queues in MSMQ upon startup, or create the schema when using a relational database as a persistence mechanism.

```sh
public class MyInstaller : INeedToInstallSomething
{
    public Task Install(string identity)
    {
        // Code to install something

        return Task.CompletedTask;
    }
}
```

Installs behave depends on how we're using this service:

- Debugging: installers will run by default every time we start a debugging session, unless we override this in the config.
- Custom installers classes should check if what we about to install is already there. 
- self hosting outside the debugger, the running of installers depends on the endpointConfiguration.EnableInstallers setting. 

```sh
endpointConfiguration.EnableInstallers();

// this will run the installers
await Endpoint.Start(endpointConfiguration)
    .ConfigureAwait(false);
```

### Retries and Fault Tolerance

The fallacies of distributed computing suggests that software and infrastructure will fail, and we want to protect ourselves against that. NServiceBus protects us against data loss and boost our system resiliency.

![pic](src/eCommerce/images/figure1.jpg)


**Happy Path flow**

1. Peeking is looking at a message without actually dequeing it.
2. If there is a message, a transaction is started, and the message is actually dequeued. NServiceBus makes sure only one thread receives the message.
3. The message is deserialized
4. The handlers (witten by us) are invoked and also everything that is rounded in the form of NServiceBus infrastructure code.
5. successful result: the transaction is committed. 


**Issue with de-serialization**

![pic](src/eCommerce/images/figure2.jpg)

3. the message is in a format that can't be deserialized : no chance that this kind of error will ever go away by itself, the message is immediately sent to the error queue 


**Issue with Handlers**

![pic](src/eCommerce/images/figure3.jpg)

4. In case of handlers transient error, i.e. which could go away by itself, e.g. called service is temporarily down, or not reachable, or a deadlock occurs in the database. NServiceBus' retry sub-process starts

![pic](src/eCommerce/images/figure4.jpg)

First NServiceBus re-invokes the handler the configured number of times (i.e. immediate retries), e.g. 5 times is the default  (i.e. everything is configurable) which might be quick for some errors. If immediate retries don't resolve the problem, delayed retries kick in. The message is moved to a special retry queue, and NServiceBus schedules the re-processing of the message in 10 seconds with five retries in a row. If the problem not solved, NServiceBus will wait 20 seconds, and again do the immediate retries 5 times, and then after 30 seconds. When all of this fails, the message is sent to the error queue. 


> if the error is indeed transient, then it won't show up in the error queue, we have to check the NServiceBus logs to see it, it will take at least a minute by default before the failed message actually appears in the error queue.

> The error queue holds messages that can't be processed, and keeps them out of the way of the normal message flow in the queues. 

> Moving the message back into the active queue can be done manually or by another process, other members of the Particular Software Suite such as ServersInsight and ServicePulse might be of some help.


### The Request/Response Pattern

This pattern sends a message with the send method, but waits for a response message to come back, this is against the nature of **NServiceBus** which handled **natively** everything **asynchronously**. It reintroduces **temporal coupling** which is an **anti-pattern**. 

> a better alternatives is using sagas with SignalR for instance.

**Example**

[PriceResponse.cs](src/eCommerce/eCommerce.Messages/PriceResponse.cs)

```sh
   public class PriceResponse: IMessage
    {
        public int Price { get; set; }
    }
```
>> note that IMessage derived from ICommand and IEvent.


[PriceRequestHandler.cs](src/eCommerce/eCommerce.Order/PriceRequestHandler.cs)

```sh
 public class PriceRequestHandler: IHandleMessages<PriceRequest>
    {
        public async Task Handle(PriceRequest message, IMessageHandlerContext context)
        {
		await context.Reply(new PriceResponse {Price = await PriceCalculator.GetPrice(message)})
		.ConfigureAwait(false);
        }
    }
```

[HomeController.cs](src/eCommerce/eCommerceUI/Controllers/HomeController.cs)

```sh
var priceResponse = await endpoint.Request<PriceResponse>(
                new PriceRequest { Weight = order.Weight }
                );
```

### Sagas

The **[NServiceBus](https://docs.particular.net/tutorials/nservicebus-sagas/1-getting-started/)** context, a **saga**'s purpose is to **coordinate** the **message flow** in a way the **business requires it**.
**Sagas** are **long-running business processes** modeled in code. They support a certain **workflow** with steps to be
executed. The saga itself **maintains state** in the form of an **object** we define until the **saga finishes**. 

![pic](src/eCommerce/images/figure5.jpg)

As long as the **saga** runs, it persists its state in a durable storage. The way sagas implementation in NServiceBus is a very open design.

#### Defining Sagas