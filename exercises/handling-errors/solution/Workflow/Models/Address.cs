namespace TemporalioHandlingErr;

public class Address
{
    required public string Line1 { get; set; }

    public string Line2 { get; set; } = string.Empty;

    required public string City { get; set; }

    required public string State { get; set; }

    required public string PostalCode { get; set; }
}