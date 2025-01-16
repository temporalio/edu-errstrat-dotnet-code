using System.Collections.ObjectModel;
using TemporalioSagaPattern.Workflow.Models;

namespace TemporalioSagaPattern;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);