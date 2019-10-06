using NServiceBus;

namespace eCommerce.Messages
{
    //Commented out in order to use polymorphism technique
    //public interface IOrderProcessedEvent : IEvent


    /*************************************************************************************
     This allow us to monitor all IOrderActivityEvent and not only the one which is processed... 
     eCommerce.OrderActivity through its OrderActivityHandler watches all activities which 
     inherits from IOrderActivityEvent base interface 
     ************************************************************************************/
    public interface IOrderProcessedEvent : IOrderActivityEvent
    {
        //All properties are inherited from IOrderActivityEvent
        //string AddressFrom { get; set; }
        //string AddressTo { get; set; }
        //int Weight { get; set; }
        //int Price { get; set; }
    }
}
