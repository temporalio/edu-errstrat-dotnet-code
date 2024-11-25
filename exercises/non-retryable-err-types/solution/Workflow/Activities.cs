using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Temporalio.Exceptions;

namespace TemporalioErrTypes;

public class Activities
{
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

            using var client = new HttpClient();

            for (var progress = startingPoint; progress <= 10; ++progress)
            {
                await Task.Delay(TimeSpan.FromSeconds(20), ctx.CancellationToken);

                ctx.Logger.LogInformation("Polling external delivery driver... Progress: {Progress}", progress);
                var response = await client.GetAsync("http://localhost:9998/getExternalDeliveryDriver", ctx.CancellationToken);

                if ((int)response.StatusCode >= 500 || (int)response.StatusCode == 403)
                {
                    throw new ApplicationFailureException($"Error. Status Code: {response.StatusCode}", nonRetryable: true);
                }

                var content = await response.Content.ReadFromJsonAsync<DeliveryResponse>(cancellationToken: ctx.CancellationToken);
                ctx.Logger.LogInformation("External delivery driver assigned from: {Service}", content?.Service);
                ctx.Heartbeat(progress);

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

        // this is a simulation, which calculates a fake (but consistent)
        // distance for a customer address based on its length. The value
        // will therefore be different when called with different addresses,
        // but will be the same across all invocations with the same address.
        var kilometers = address.Line1.Length + address.Line2.Length - 10;
        if (kilometers < 1)
        {
            kilometers = 5;
        }

        var distance = new Distance
        {
            Kilometers = kilometers,
        };

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

        // reject invalid amounts before calling the payment processor
        if (chargeAmount < 0)
        {
            throw new ApplicationFailureException($"invalid charge amount: {chargeAmount} (must be above zero)", details: new[] { bill }, nonRetryable: true);
        }

        // pretend we called a payment processing service here :-)
        var confirmation = new OrderConfirmation
        {
            OrderNumber = bill.OrderNumber,
            ConfirmationNumber = "AB9923",
            Status = "SUCCESS",
            BillingTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = chargeAmount,
        };

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
            throw new ApplicationFailureException("Invalid credit card number: must contain exactly 16 digits", details: new[] { creditCardNumber }, errorType: "InvalidCreditCardErr");
        }

        return Task.CompletedTask;
    }
}