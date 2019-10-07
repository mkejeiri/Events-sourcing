using System;
using System.Net;
using System.Web.Http;
using Payments.Models;
using Payments.RabbitMQ;

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
