using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;

        public TransactionsController(ITransactionService transactionService, IClientService clientService, IAccountService accountService)
        {
            _transactionService = transactionService;
            _clientService = clientService;
            _accountService = accountService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                // Validaciones
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    return NotFound("Client not found.");
                }
                var client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

                if (transferDTO.FromAccountNumber == string.Empty || transferDTO.ToAccountNumber == string.Empty)
                {
                    return StatusCode(403, "Origin or destination account needed");
                }

                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                {
                    return StatusCode(403, "Transfer to same account not allowed.");
                }

                if (transferDTO.Amount <= 0 || transferDTO.Amount < 0.001)
                {
                    return StatusCode(403, "Invalid ammount");
                }

                if (transferDTO.Description == string.Empty)
                {
                    return StatusCode(403, "Description needed.");
                }

                Account fromAccount = _accountService.GetAccountByNumber(transferDTO.FromAccountNumber);
                if (fromAccount == null)
                {
                    return StatusCode(403, "Origin account does not exists");
                }

                if (fromAccount.Balance < transferDTO.Amount)
                {
                    return StatusCode(403, "insufficient funds");
                }

                Account toAccount = _accountService.GetAccountByNumber(transferDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return StatusCode(403, "Destination account does not exists");
                }

                if (transferDTO.Type == "own" && !(fromAccount.Client == toAccount.Client))
                {
                    return StatusCode(403, "Las cuentas deben pertenecer al mismo cliente");
                }

                if (transferDTO.Type == "third" && fromAccount.Client == toAccount.Client)
                {
                    return StatusCode(403, "Las cuentas deben pertenecer a diferentes clientes");
                }

                _transactionService.MakeTransfer(transferDTO);

                return Ok("Created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
