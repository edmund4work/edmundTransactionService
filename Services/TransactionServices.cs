using System.Globalization;
using System.Xml;
using CsvHelper;
using edmundTransactionService.Data;
using edmundTransactionService.Services.Interface;
using edmundTransactionService.Models;

namespace edmundTransactionService.Services;

public class TransactionServices : ITransactionServices
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ApplicationDbContext _dbContext;
    public TransactionServices(ILogger<TransactionController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public returnDataResults GetTransactions(string currency = null, DateTime? startDate = null, DateTime? endDate = null, string status = null)
    {
        returnDataResults returnData = new returnDataResults();
        bool success = true;
        var query = _dbContext.Transactions.AsQueryable();

        // Apply filters based on the provided criteria
        if (!string.IsNullOrEmpty(currency))
        {
            query = query.Where(t => t.CurrencyCode == currency);
        }
        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        }
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status == MapStatus(status));
        }

        var transactions = query.Select(t => new
        {
            id = t.Id,
            payment = $"{t.Amount} {t.CurrencyCode}",
            status = MapStatus(t.Status)
        }).ToList();

        returnData.success = true;
        returnData.messages = "Data Retrieved";
        returnData.results = transactions;

        return returnData;
    }
    public returnDataResults UploadFile(IFormFile file)
    {
        returnDataResults returnData = new returnDataResults();
        returnData.success = false;
        try
        {
            if (file == null || file.Length == 0)
            {
                returnData.messages = "No file uploaded.";
                return returnData;
            }

            // Determine the file format based on the file extension or content
            var format = GetFileFormat(file.FileName);
            if (format == FileFormat.Unknown)
            {
                returnData.messages = "Unknown file format.";
                return returnData;
            }

            // Parse and validate the file
            var transactions = ParseFile(file, format);
            if (transactions == null)
            {
                returnData.messages = "File validation failed.";
                return returnData;
            }

            // Save the transactions to the database
            _dbContext.Transactions.AddRange(transactions);
            _dbContext.SaveChanges();

            returnData.success = true;
            returnData.messages = "File Uploaded";
        }
        catch (Exception ex)
        {
            returnData.messages = ex.Message;
        }
        return returnData;
    }
    private FileFormat GetFileFormat(string fileName)
    {
        // Determine the file format based on the file extension or content
        if (fileName.EndsWith(".csv"))
        {
            return FileFormat.Csv;
        }
        else if (fileName.EndsWith(".xml"))
        {
            return FileFormat.Xml;
        }
        else
        {
            return FileFormat.Unknown;
        }
    }
    private List<Transaction> ParseFile(IFormFile file, FileFormat format)
    {
        // Parse and validate the file content based on the format
        switch (format)
        {
            case FileFormat.Csv:
                return ParseCsvFile(file);
            case FileFormat.Xml:
                return ParseXmlFile(file);
            default:
                return null;
        }
    }

    private List<Transaction> ParseCsvFile(IFormFile file)
    {
        var transactions = new List<Transaction>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // csv.Configuration.RegisterClassMap<TransactionCsvMap>();
            csv.Context.RegisterClassMap<TransactionCsvMap>();
            try
            {
                transactions = csv.GetRecords<Transaction>().ToList();
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex, "CSV parsing error");
                return null;
            }
        }

        return transactions;
    }

    private List<Transaction> ParseXmlFile(IFormFile file)
    {
        var transactions = new List<Transaction>();

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(file.OpenReadStream());

            var transactionNodes = xmlDoc.SelectNodes("/Transactions/Transaction");
            foreach (XmlNode node in transactionNodes)
            {
                var transaction = new Transaction
                {
                    Id = node.Attributes["id"].Value,
                    Amount = decimal.Parse(node.SelectSingleNode("PaymentDetails/Amount").InnerText),
                    CurrencyCode = node.SelectSingleNode("PaymentDetails/CurrencyCode").InnerText,
                    TransactionDate = DateTime.Parse(node.SelectSingleNode("TransactionDate").InnerText),
                    Status = node.SelectSingleNode("Status").InnerText
                };

                transactions.Add(transaction);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XML parsing error");
            return null;
        }

        return transactions;
    }

    private string MapStatus(string status)
    {
        // Map the transaction status from CSV and XML formats to the unified format
        switch (status.ToLower())
        {
            case "approved":
                return "A";
            case "failed":
                return "R";
            case "finished":
                return "D";
            case "rejected":
                return "R";
            case "done":
                return "D";
            default:
                return "Unknown";
        }
    }
}

public enum FileFormat
{
    Unknown,
    Csv,
    Xml
}