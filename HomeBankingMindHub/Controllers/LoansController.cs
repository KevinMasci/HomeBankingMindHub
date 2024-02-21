using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;

        public LoansController(ILoanService loanService, IAccountService accountService, IClientService clientService)
        {
            _loanService = loanService;
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanService.GetAll();

                // Verificar si hay préstamos disponibles
                if (loans == null)
                {
                    return NotFound("No loans available");
                }
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanAppDto)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("Client not found.");
                }

                // Verificar que el préstamo exista
                var loan = _loanService.FindById(loanAppDto.LoanId);
                if (loan == null)
                {
                    return NotFound("Loan does not exist");
                }

                // Verificar que el monto no sea 0 y no sobrepase el máximo autorizado
                if (loanAppDto.Amount <= 0 || loanAppDto.Amount > loan.MaxAmount)
                {
                    return BadRequest("Invalid loan amount");
                }

                // Verificar que los payments no lleguen vacíos
                if (string.IsNullOrEmpty(loanAppDto.Payments))
                {
                    return BadRequest("Payments information is required");
                }

                // Verifica que el número de cuotas seleccionado esté entre los valores permitidos
                var paymentValues = loan.Payments.Split(',').Select(s => s.Trim()).ToList();
                if (!paymentValues.Contains(loanAppDto.Payments.ToString()))
                {
                    return BadRequest("Invalid number of payments");
                }

                // Verificar que exista la cuenta de destino
                var acc = _accountService.GetAccountByNumber(loanAppDto.ToAccountNumber);
                if (acc == null)
                {
                    return NotFound("Destination account does not exist");
                }

                // Verificar que la cuenta destino pertenezca al cliente autenticado
                var client = _clientService.FindByEmail(email);
                if (acc.ClientId != client.Id)
                {
                    return BadRequest("Destination account does not belong to the authenticated client.");
                }

                var loanRequested = _loanService.RequestLoan(loanAppDto, email);
                return Ok(loanRequested);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
