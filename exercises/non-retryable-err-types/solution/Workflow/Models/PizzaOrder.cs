using System.Collections.ObjectModel;
using Temporalio.NonRetryableErrTypes.Workflow.Models;

namespace TemporalioNonRetryableErrTypes;

public record PizzaOrder(
    string OrderNumber,
    Customer Customer,
    Collection<Pizza> Items,
    Address Address,
    bool IsDelivery = false);