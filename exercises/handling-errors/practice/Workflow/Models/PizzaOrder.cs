using System.Collections.ObjectModel;
using TemporalioHandlingErrors.Practice.Workflow.Models;

namespace TemporalioHandlingErrors.Practice.Workflow;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);