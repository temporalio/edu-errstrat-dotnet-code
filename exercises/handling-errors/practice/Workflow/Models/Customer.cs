namespace TemporalioHandlingErr;

public class Customer
{
    required public int CustomerId { get; set; }

    required public string Name { get; set; }

    required public string Email { get; set; }

    required public string Phone { get; set; }

    public string CreditCardNumber { get; set; } = string.Empty;
}