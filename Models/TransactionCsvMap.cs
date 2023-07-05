using CsvHelper.Configuration;

namespace edmundTransactionService.Models;
public sealed class TransactionCsvMap : ClassMap<Transaction>
{
    public TransactionCsvMap()
    {
        Map(m => m.Id).Name("Transaction Identificator");
        Map(m => m.Amount).Name("Amount");
        Map(m => m.CurrencyCode).Name("Currency Code");
        Map(m => m.TransactionDate).Name("Transaction Date");
        Map(m => m.Status).Name("Status");
    }
}
