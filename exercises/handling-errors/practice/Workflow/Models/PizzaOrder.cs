using System.Collections.ObjectModel;
using Temporalio.HandlingErrors.Workflow.Models;

namespace TemporalioHandlingErrors;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);