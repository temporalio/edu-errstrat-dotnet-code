namespace Temporalio.SagaPattern.Workflow.Models;

public record Bill(
    int CustomerId,
    string OrderNumber,
    string Description,
    int Amount);