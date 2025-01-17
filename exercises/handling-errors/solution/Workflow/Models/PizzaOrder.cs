using System.Collections.ObjectModel;
using TemporalioHandlingErrors.Solution.Workflow.Models;

namespace TemporalioHandlingErrors.Solution.Workflow;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);