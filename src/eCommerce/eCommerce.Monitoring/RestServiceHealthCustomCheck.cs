using System;
using System.Threading.Tasks;
using ServiceControl.Plugin.CustomChecks;

namespace eCommerce.Monitoring
{
    //Implementing CustomChecks
    //1- add RestServiceHealthCustomCheck
    //2- add ServiceControl.Plugin.Nsb6.CustomChecks nuget package
    //3- RestServiceHealthCustomCheck derive from CustomCheck base class
    //4- it will run every time your services start, but optionally indicate a time interval
    //The check will run continuously with the indicated time between runs
    //5-Implement the actual check in the PerformCheck method
    public class RestServiceHealthCustomCheck: CustomCheck
    {
     public RestServiceHealthCustomCheck(): 
            base("RestServiceHealth", //giving the CustomCheck an ID
                "RestService", // a category
                TimeSpan.FromSeconds(5)) //specify the time interval
        { }

        public override Task<CheckResult> PerformCheck()
        {
            //code: try Ping service

            //CheckResult.Pass or CheckResult.Failed with an indicator of why it failed
            //so can see the custom check failing on the ServicePulse dashboard
            return CheckResult.Failed("REST service not reachable");
        }
    }
}