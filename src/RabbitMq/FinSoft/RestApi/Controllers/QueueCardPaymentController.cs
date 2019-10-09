using System;
using System.Net;
using System.Web.Http;
using Payments.Models;
using Payments.RabbitMQ;
/*
 This controller also has a Make Payment method, that will post a card payment request onto the
 queue for processing by another system. But in normal operation, we expect this message to be 
 picked up and processed quite quickly. There's no guarantee that this will happen, so the client
 should not sit there and wait for any kind of response.
 */
namespace Payments.Controllers
{
    public class QueueCardPaymentController : ApiController
    {
        [HttpPost]
        public IHttpActionResult MakePayment([FromBody] CardPayment payment)
        {
            try
            {
                RabbitMQClient client = new RabbitMQClient();
                client.SendPayment(payment);

                //RabbitMQ closes the connection
                client.Close();
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            return Ok(payment);
        }
    }
}
