using System;
using System.Net;
using System.Web.Http;
using Payments.Models;
using Payments.RabbitMQ;

/*
 This API has a Make Payment method that posts a purchase order request onto the queue, 
 As with the Queue Card Payment API, there is no guarantee that this message will be picked up 
 immediately and processed, so the client should not wait for a response.
 */
namespace Payments.Controllers
{
    public class QueuePurchaseOrderController : ApiController
    {
        [HttpPost]
        public IHttpActionResult MakePayment([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                RabbitMQClient client = new RabbitMQClient();
                client.SendPurchaseOrder(purchaseOrder);
                client.Close();
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            return Ok(purchaseOrder);
        }
    }
}
