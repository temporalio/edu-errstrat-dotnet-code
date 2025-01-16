namespace TemporalioHandlingErrors;

using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.HandlingErrors.Workflow.Models;
using Temporalio.Workflows;

[Workflow]
public class PizzaWorkflow
{
    [WorkflowRun]
    public async Task<OrderConfirmation> RunAsync(PizzaOrder order)
    {
        var logger = Workflow.Logger;
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(5),
            RetryPolicy = new() { MaximumInterval = TimeSpan.FromSeconds(10) },
        };

        try
        {
            await Workflow.ExecuteActivityAsync(
                (Activities act) => act.ValidateCreditCardAsync(order.Customer.CreditCardNumber),
                options);
        }
        catch (ActivityFailureException err)
        {
            logger.LogError("Unable to process credit card: {Message}", err.Message);
            throw new ApplicationFailureException(
                message: "Invalid credit card number error",
                details: new[] { order.Customer.CreditCardNumber });
        }

        if (order.IsDelivery)
        {
            try
            {
                var distance = await Workflow.ExecuteActivityAsync(
                    (Activities act) => act.GetDistanceAsync(order.Address),
                    options);

                if (distance.Kilometers > 25)
                {
                    throw new ApplicationFailureException(
                        message: "Customer lives too far away for delivery",
                        details: new[] { $"Distance: {distance.Kilometers}km" });
                }
            }
            catch (ActivityFailureException err)
            {
                logger.LogError("Unable to get distance: {Message}", err.Message);
                throw new ApplicationFailureException(
                    message: "Problem with delivery",
                    details: new[] { err.Message });
            }
        }

        var totalPrice = order.Items.Sum(pizza => pizza.Price);

        await Workflow.DelayAsync(TimeSpan.FromSeconds(3));

        var bill = new Bill(
            CustomerId: order.Customer.CustomerId,
            OrderNumber: order.OrderNumber,
            Description: "Pizza",
            Amount: totalPrice);

        try
        {
            return await Workflow.ExecuteActivityAsync((Activities act) => act.SendBillAsync(bill), options);
        }
        catch (Exception err)
        {
            logger.LogError("Unable to bill customer: {Message}", err.Message);
            throw new ApplicationFailureException(
                message: "Unable to bill customer",
                details: new[] { err.Message });
        }
    }
}