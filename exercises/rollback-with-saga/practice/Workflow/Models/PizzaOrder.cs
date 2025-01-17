namespace TemporalioSagaPattern.Practice.Workflow;

using System.Collections.ObjectModel;
using TemporalioSagaPattern.Practice.Workflow.Models;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);