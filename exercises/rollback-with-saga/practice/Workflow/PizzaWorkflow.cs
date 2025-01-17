namespace TemporalioSagaPattern.Practice.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Workflows;
using TemporalioSagaPattern.Practice.Workflow.Models;

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

            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.UpdateInventoryAsync(order.Items),
                options);
            compensations.Add(async () => await Workflow.ExecuteActivityAsync(
                (Activities act) => act.RevertInventoryAsync(order.Items),
                options));

            var confirmation = await Workflow.ExecuteActivityAsync(
                (Activities act) => act.SendBillAsync(bill),
                options);
            // TODO PART B: Add a compensating action for the `SendBill` Activity.
            // Pass the bill as the input.
            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.ValidateCreditCardAsync(order.Customer.CreditCardNumber),
                options);

            return confirmation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Workflow failed, initiating compensation");
            // TODO Part D: Call `await CompensateAsync()`
            throw;
        }
    }

    private async Task CompensateAsync()
    {
        // TODO Part C: Call the `Reverse` method on your compensations list.
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