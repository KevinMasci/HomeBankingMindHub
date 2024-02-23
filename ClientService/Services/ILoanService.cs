using ClientService.DTO;
using ClientService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.Services
{
    public interface ILoanService
    {
        IEnumerable<LoanDTO> GetAll();
        Loan FindById(long id);
        ClientLoanDTO RequestLoan(LoanApplicationDTO loanAppDto, string email);
        ClientLoan GetClientLoanByEmailAndType(string email, long LoanId);
    }
}
