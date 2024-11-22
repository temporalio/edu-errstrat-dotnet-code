using System.Collections.ObjectModel;

namespace TemporalioSagaPattern;

public class PizzaOrder
{
    required public string OrderNumber { get; set; }

    required public Customer Customer { get; set; }

    required public Collection<Pizza> Items { get; init; }

    required public bool IsDelivery { get; set; }

    required public Address Address { get; set; }
}