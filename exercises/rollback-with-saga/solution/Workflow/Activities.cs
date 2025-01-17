namespace TemporalioSagaPattern.Solution.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Exceptions;
using TemporalioSagaPattern.Solution.Workflow.Models;

public class Activities
{
    [Activity]
    public Task<Distance> GetDistanceAsync(Address address)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("GetDistance invoked; determining distance to customer address");

        // This is a simulation, which calculates a fake (but consistent)
        // distance for a customer address based on its length. The value
        // will therefore be different when called with different addresses,
        // but will be the same across all invocations with the same address.
        var kilometers = address.Line1.Length + address.Line2.Length - 10;
        if (kilometers < 1)
        {
            kilometers = 5;
        }

        var distance = new Distance(kilometers);

        logger.LogInformation("GetDistance complete. Distance: {Distance}", distance.Kilometers);
        return Task.FromResult(distance);
    }

    [Activity]
    public Task ValidateCreditCardAsync(string creditCardNumber)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("ValidateCreditCard invoked {Amount}", creditCardNumber);

        if (creditCardNumber.Length != 16)
        {
            throw new ApplicationFailureException("Invalid credit card number: must contain exactly 16 digits", nonRetryable: true);
        }

        return Task.CompletedTask;
    }

    [Activity]
    public Task UpdateInventoryAsync(ICollection<Pizza> items)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        // Here you would call your inventory management system to reduce the stock of your pizza inventory
        logger.LogInformation("Updating inventory for {Count} items", items.Count);
        return Task.FromResult("Updated inventory");
    }

    [Activity]
    public Task RevertInventoryAsync(ICollection<Pizza> items)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        // Here you would call your inventory management system to add the ingredients back into the pizza inventory.
        logger.LogInformation("Reverting inventory for {Count} items", items.Count);
        return Task.FromResult("Reverted inventory");
    }

    [Activity]
    public Task RefundCustomerAsync(Bill bill)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        // Simulate refunding the customer
        logger.LogInformation(
            "Refunding {Amount} to customer {CustomerId} for order {OrderNumber}",
            bill.Amount,
            bill.CustomerId,
            bill.OrderNumber);
        return Task.FromResult("Refunded {Amount} to customer {CustomerId} for order {OrderNumber}");
    }

    [Activity]
    public Task<OrderConfirmation> SendBillAsync(Bill bill)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("SendBill invoked. Customer: {Customer}, Amount: {Amount}", bill.CustomerId, bill.Amount);

        var chargeAmount = bill.Amount;

        if (bill.Amount > 3000)
        {
            logger.LogInformation("Applying discount");
            chargeAmount -= 500;
        }

        if (chargeAmount < 0)
        {
            throw new ArgumentException($"Invalid charge amount: {chargeAmount} (must be above zero)");
        }

        var confirmation = new OrderConfirmation(
             OrderNumber: bill.OrderNumber,
             Status: "SUCCESS",
             ConfirmationNumber: "AB9923",
             BillingTimestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
             Amount: chargeAmount);

        logger.LogInformation("SendBill complete. ConfirmationNumber: {Confirmation}", confirmation.ConfirmationNumber);

        return Task.FromResult(confirmation);
    }
}