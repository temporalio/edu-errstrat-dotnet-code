namespace Temporalio.HandlingErrors.Workflow.Models;

public record OrderConfirmation(
    string OrderNumber,
    string Status,
    string ConfirmationNumber,
    long BillingTimestamp,
    int Amount);