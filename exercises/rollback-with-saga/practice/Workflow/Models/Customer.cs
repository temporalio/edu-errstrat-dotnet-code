namespace TemporalioSagaPattern.Practice.Workflow.Models;

public record Customer(
    int CustomerId,
    string Name,
    string Phone,
    string CreditCardNumber,
    string Email = "");