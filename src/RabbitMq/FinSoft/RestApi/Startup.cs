using System.Web.Http;
using Payments.App_Start;
using Owin;

namespace Payments
{
    public class Startup
    {
        public void Configuration(IAppBuilder appbuilder)
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            appbuilder.UseWebApi(httpConfiguration);
        }
    }
}
