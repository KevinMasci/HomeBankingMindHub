using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
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
                var loanRequested = _loanService.RequestLoan(loanAppDto, email);
                return Ok(loanRequested);
            }
            catch(InvalidOperationException ex)
            {
                return StatusCode(400, ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
