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
        public async  Task<IActionResult> Get()
        {
            try
            {
                var result = await _loanService.GetAll();

                return result;
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Internal server error: {ex.Message}") { StatusCode = 500 };
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoanApplicationDTO loanAppDto)
        {
            string email = User.FindFirst("Client")?.Value;
            return await _loanService.RequestLoan(loanAppDto, email);
        }
    }
}
