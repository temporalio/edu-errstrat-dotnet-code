namespace TemporalioSagaPattern.Solution.Workflow;

using System.Collections.ObjectModel;
using TemporalioSagaPattern.Solution.Workflow.Models;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);