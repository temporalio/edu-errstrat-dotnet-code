namespace TemporalioHandlingErrors.Practice.Workflow.Models;

public record Address(
    string Line1,
    string City,
    string State,
    string PostalCode,
    string Line2 = "");