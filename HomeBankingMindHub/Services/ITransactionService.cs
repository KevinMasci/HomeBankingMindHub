using HomeBankingMindHub.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        void MakeTransfer(TransferDTO transferDTO);
    }
}
