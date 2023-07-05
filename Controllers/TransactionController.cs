using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TransactionController(ILogger<TransactionController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost("upload")]
    public IActionResult UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Determine the file format based on the file extension or content
        var format = GetFileFormat(file.FileName);
        if (format == FileFormat.Unknown)
        {
            return BadRequest("Unknown file format.");
        }

        // Parse and validate the file
        var transactions = ParseFile(file, format);
        if (transactions == null)
        {
            return BadRequest("File validation failed.");
        }

        // Save the transactions to the database
        _dbContext.Transactions.AddRange(transactions);
        _dbContext.SaveChanges();

        return Ok("File uploaded successfully.");
    }

    [HttpGet]
    public IActionResult GetTransactions(string currency = null, DateTime? startDate = null, DateTime? endDate = null, string status = null)
    {
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

        return Ok(transactions);
    }