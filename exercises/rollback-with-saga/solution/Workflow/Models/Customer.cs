namespace Temporalio.SagaPattern.Workflow.Models;

public record Customer(
    int CustomerId,
    string Name,
    string Phone,
    string Email = "",
    string CreditCardNumber = "");