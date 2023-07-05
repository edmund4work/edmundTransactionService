using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using edmundTransactionService.Services.Interface;
using edmundTransactionService.Models;

[ApiController, Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private static ITransactionServices _transactionServices;

    public TransactionController(ITransactionServices transactionServices)
    {
        _transactionServices = transactionServices;
    }

    [HttpPost("upload")]
    public IActionResult UploadFile(IFormFile file)
    {
        returnDataResults returnResult = _transactionServices.UploadFile(file);
        if (returnResult != null)
        {
            if (returnResult.success)
                return Ok(returnResult.messages);
            else
                return BadRequest(returnResult.messages);
        }
        else
            return BadRequest("Unknow Error.");
    }

    [HttpGet]
    public IActionResult GetTransactions(string currency = null, DateTime? startDate = null, DateTime? endDate = null, string status = null)
    {
        returnDataResults returnResult = _transactionServices.GetTransactions(currency, startDate, endDate, status);
        if (returnResult != null)
        {
            if (returnResult.success)
                return Ok(returnResult.messages);
            else
                return BadRequest(returnResult.messages);
        }
        else
            return BadRequest("Unknow Error.");
    }
}
