using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        Task<IActionResult> GetAll();
        Loan FindById(long id);
        Task<IActionResult> RequestLoan(LoanApplicationDTO loanAppDto, string email);
    }
}
