using ClientService.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.Services
{
    public interface ITransactionService
    {
        void MakeTransfer(TransferDTO transferDTO);
    }
}
