namespace TemporalioNonRetryableErrTypes.Solution.Workflow;

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Exceptions;
using TemporalioNonRetryableErrTypes.Solution.Workflow.Models;

public class Activities
{
    private static readonly HttpClient HttpClient = new();

    [Activity]
    public static async Task PollDeliveryDriverAsync(PizzaOrder order)
    {
        var ctx = ActivityExecutionContext.Current;
        try
        {
            // Allow for resuming from heartbeat
            var startingPoint = ctx.Info.HeartbeatDetails.Count > 0
                ? await ctx.Info.HeartbeatDetailAtAsync<int>(0)
                : 1;

            ctx.Logger.LogInformation("Starting delivery driver polling at progress: {StartingPoint}", startingPoint);

            for (var progress = startingPoint; progress <= 10; ++progress)
            {
                await Task.Delay(TimeSpan.FromSeconds(20), ctx.CancellationToken);

                ctx.Logger.LogInformation("Polling external delivery driver... Progress: {Progress}", progress);
                using var jsonContent = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync("http://localhost:9998/findExternalDeliveryDriver", jsonContent, ctx.CancellationToken);

                if ((int)response.StatusCode >= 500 || (int)response.StatusCode == 403)
                {
                    // TODO Part C: Throw a new `ApplicationFailureException` with a message that lets
                    // the user know that there is an invalid server error.
                    // Set the `nonRetryable` key to true.
                }

                var deliveryResponse = await response.Content.ReadFromJsonAsync<DeliveryResponse>(cancellationToken: ctx.CancellationToken);
                ctx.Logger.LogInformation("External delivery driver assigned from: {Service}", deliveryResponse?.Service);
                // TODO Part C: Call `ctx.Heartbeat()` taking in `progress`.
                if (response.IsSuccessStatusCode)
                {
                    break;
                }
            }

            ctx.Logger.LogInformation("Delivery driver polling completed successfully");
        }
        catch (OperationCanceledException)
        {
            ctx.Logger.LogInformation("Delivery driver polling cancelled");
            throw;
        }
        catch (Exception ex)
        {
            ctx.Logger.LogError(ex, "External delivery driver request failed");
            throw;
        }
    }

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
            throw new ApplicationFailureException($"Invalid charge amount: {chargeAmount} (must be above zero)", details: new[] { bill }, nonRetryable: true);
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
            // TODO Part A: Add an `errorType` key and set it to a string: `InvalidCreditCardErr`. Remove the `nonRetryable` key.
            throw new ApplicationFailureException("Invalid credit card number: must contain exactly 16 digits", details: new[] { creditCardNumber }, nonRetryable: true);
        }

        return Task.CompletedTask;
    }
}