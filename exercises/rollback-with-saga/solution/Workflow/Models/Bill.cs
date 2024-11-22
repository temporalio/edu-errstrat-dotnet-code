namespace TemporalioSagaPattern;

public class Bill
{
    required public int CustomerId { get; set; }

    required public string OrderNumber { get; set; }

    required public string Description { get; set; }

    required public int Amount { get; set; }
}