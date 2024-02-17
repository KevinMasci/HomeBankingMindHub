using HomeBankingMindHub.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        IActionResult MakeTransfer(TransferDTO transferDTO, string userEmail);
    }
}
