namespace TemporalioSagaPattern.Solution.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Workflows;
using TemporalioSagaPattern.Solution.Workflow.Models;

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
            var bill = new Bill(
                CustomerId: order.Customer.CustomerId,
                OrderNumber: order.OrderNumber,
                Description: "Pizza",
                Amount: totalPrice);

            // Register compensation for inventory update before executing the Activity
            compensations.Add(() => Workflow.ExecuteActivityAsync(
                (Activities act) => act.RevertInventory(order.Items),
                options));
            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.UpdateInventory(order.Items),
                options);

            // Register compensation for billing before executing the Activity
            compensations.Add(() => Workflow.ExecuteActivityAsync(
                (Activities act) => act.RefundCustomer(bill),
                options));
            var confirmation = await Workflow.ExecuteActivityAsync(
                (Activities act) => act.SendBillAsync(bill),
                options);

            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.ValidateCreditCard(order.Customer.CreditCardNumber),
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