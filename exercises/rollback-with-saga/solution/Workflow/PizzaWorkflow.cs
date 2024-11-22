using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace TemporalioSagaPattern;

[Workflow]
public class PizzaWorkflow
{
    private readonly List<Func<Task>> compensations = new();

    [WorkflowRun]
    public async Task<OrderConfirmation> RunAsync(PizzaOrder order)
    {
        var logger = Workflow.Logger;
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(5),
            RetryPolicy = new()
            {
                MaximumInterval = TimeSpan.FromSeconds(10),
            },
        };

        try
        {
            if (order.IsDelivery)
            {
                var distance = await Workflow.ExecuteActivityAsync((Activities act) => act.GetDistanceAsync(order.Address), options);

                if (distance.Kilometers > 25)
                {
                    throw new ApplicationFailureException("Customer lives too far away for delivery");
                }
            }

            await Workflow.DelayAsync(TimeSpan.FromSeconds(3));

            var totalPrice = order.Items.Sum(pizza => pizza.Price);
            var bill = new Bill
            {
                CustomerId = order.Customer.CustomerId,
                OrderNumber = order.OrderNumber,
                Amount = totalPrice,
                Description = "Pizza",
            };

            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.UpdateInventoryAsync(order.Items),
                options);
            compensations.Add(async () => await Workflow.ExecuteActivityAsync(
                (Activities act) => act.RevertInventoryAsync(order.Items),
                options));

            var confirmation = await Workflow.ExecuteActivityAsync(
                (Activities act) => act.SendBillAsync(bill),
                options);
            compensations.Add(async () => await Workflow.ExecuteActivityAsync(
                (Activities act) => act.RefundCustomerAsync(bill),
                options));

            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.ValidateCreditCardAsync(order.Customer.CreditCardNumber),
                options);

            return confirmation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Workflow failed, initiating compensation");
            await CompensateAsync();
            throw;
        }
    }

    private async Task CompensateAsync()
    {
        compensations.Reverse();
        var logger = Workflow.Logger;
        foreach (var compensation in compensations)
        {
            try
            {
                await compensation.Invoke();
            }
            catch (ApplicationFailureException ex)
            {
                logger.LogError(ex, "Compensation failed");
            }
        }
    }
}