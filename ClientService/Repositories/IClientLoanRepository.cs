using ClientService.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClientService.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
        ClientLoan GetClientLoanByEmailAndType(long clientId, long loanId);
    }
}
