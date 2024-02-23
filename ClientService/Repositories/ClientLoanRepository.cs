using ClientService.Data;
using ClientService.Models;

namespace ClientService.Repositories
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public void Save(ClientLoan clientLoan)
        {
            Create(clientLoan);
            SaveChanges();
        }

        public ClientLoan GetClientLoanByEmailAndType(long clientId, long loanId) 
        {
            return FindByCondition(clientLoan => clientLoan.ClientId == clientId && clientLoan.LoanId == loanId).FirstOrDefault();
        }
    }
}
