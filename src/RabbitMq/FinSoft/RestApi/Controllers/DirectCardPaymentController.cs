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

