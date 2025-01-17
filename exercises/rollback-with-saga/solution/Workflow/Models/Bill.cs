namespace TemporalioSagaPattern.Solution.Workflow.Models;

public record Bill(
    int CustomerId,
    string OrderNumber,
    string Description,
    int Amount);