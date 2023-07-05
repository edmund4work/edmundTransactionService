
using edmundTransactionService.Models;

namespace edmundTransactionService.Services.Interface;

public interface ITransactionServices
{
    public returnDataResults GetTransactions(string currency = null, DateTime? startDate = null, DateTime? endDate = null, string status = null);
    public returnDataResults UploadFile(IFormFile file);
}