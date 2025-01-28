namespace TemporalioHandlingErrors.Practice.Workflow;

using Microsoft.Extensions.Logging;
using Temporalio.Activities;
// TODO Part A: Add `Temporalio.Exceptions` to your imports
using TemporalioHandlingErrors.Practice.Workflow.Models;

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

        logger.LogDebug("GetDistance complete. Distance: {Distance}", distance.Kilometers);
        return Task.FromResult(distance);
    }

    [Activity]
    public Task<OrderConfirmation> SendBillAsync(Bill bill)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("SendBill invoked. Customer: {Customer}, Amount: {Amount}", bill.CustomerId, bill.Amount);

        var chargeAmount = bill.Amount;

        // This month's special offer: Get $5 off all orders over $30
        if (bill.Amount > 3000)
        {
            logger.LogInformation("Applying discount");
            chargeAmount -= 500;
        }

        // Reject invalid amounts before calling the payment processor
        if (chargeAmount < 0)
        {
            // TODO Part A: Set the nonRetryable key to true in the Application Failure
            throw new ApplicationFailureException($"Invalid charge amount: {chargeAmount} (must be above zero)", details: new[] { bill });
        }

        // Pretend we called a payment processing service here :-)
        var confirmation = new OrderConfirmation(
            OrderNumber: bill.OrderNumber,
            Status: "SUCCESS",
            ConfirmationNumber: "AB9923",
            BillingTimestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount: chargeAmount);

        logger.LogDebug("SendBill complete. ConfirmationNumber: {Confirmation}", confirmation.ConfirmationNumber);
        return Task.FromResult(confirmation);
    }

    [Activity]
    public Task ValidateCreditCardAsync(string creditCardNumber)
    {
        var logger = ActivityExecutionContext.Current.Logger;
        logger.LogInformation("ValidateCreditCard invoked {Amount}", creditCardNumber);

        if (creditCardNumber.Length != 16)
        {
            // TODO Part A: Throw an `ApplicationFailureException`.
            // In the details field, add your credit card number.
            // Set the nonRetryable key to true.
        }

        return Task.CompletedTask;
    }
}