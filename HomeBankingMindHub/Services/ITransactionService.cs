using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        IActionResult MakeTransfer(TransferDTO transferDTO, string userEmail);
    }
}
