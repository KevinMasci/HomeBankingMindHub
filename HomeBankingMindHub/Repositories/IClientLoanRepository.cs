using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HomeBankingMindHub.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
