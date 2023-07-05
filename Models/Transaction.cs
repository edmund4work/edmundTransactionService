namespace edmundTransactionService.Models;
public class Transaction
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }
}

public class returnDataResults
{
    public bool success { get; set; }
    public string messages { get; set; }
    public object results { get; set; }
}