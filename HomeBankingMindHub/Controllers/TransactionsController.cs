using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    return NotFound("Client not found.");
                }

                return _transactionService.MakeTransfer(transferDTO, email);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
