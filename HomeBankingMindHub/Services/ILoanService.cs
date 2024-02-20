using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<LoanDTO> GetAll();
        Loan FindById(long id);
        ClientLoanDTO RequestLoan(LoanApplicationDTO loanAppDto, string email);
    }
}
