namespace TemporalioHandlingErr;

public class OrderConfirmation
{
    required public string OrderNumber { get; set; }

    required public string Status { get; set; }

    required public string ConfirmationNumber { get; set; }

    required public long BillingTimestamp { get; set; }

    required public int Amount { get; set; }
}