using System;
using System.Net;
using System.Web.Http;
using Payments.Models;
using Payments.RabbitMQ;
/*
     Direct Card API controller. This has a method called Make Payment that will take a card payment. 
     This method is not asynchronous in nature as a client will wait for a reply
     the call center needs the ability to take payments when the customer is on the phone.
 */
namespace Payments.Controllers
{
    public class DirectCardPaymentController : ApiController
    {
        [HttpPost]
        public IHttpActionResult MakePayment([FromBody] CardPayment payment)
        {
            string reply;

            try
            {
                /*
                 We could just call in to the back end payment systems directly, 
                 but we still want to use messages to flow through the RabbitMQ queues. 
                 This will be achieved by using the remote procedure called Pattern
                 */

                RabbitMQDirectClient client = new RabbitMQDirectClient();
                client.CreateConnection();
                reply = client.MakePayment(payment);

                client.Close();
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            return Ok(reply);
        }
    }
}
/*
 The client application posts messages directly onto a queue. 
 For each message that gets posted, the application waits for a reply from a reply queue. 
 This essentially makes this a synchronous process. When a message is posted to the server from the client, 
 a correlation ID is generated and attached to the message properties. 
 The same correlation ID is put onto the properties in a reply message. 
 This is  useful, as it allows you to easily tie together the replies in 
 the original 18 messages if you store them for retrieval later.
 The client posts a message to the RPC queue that has a correlation ID of e.g. 12345. 
 This message is received by the server and a reply is sent back to the client 
 on a reply queue with the same correlation ID of e.g 12345. 
 */
