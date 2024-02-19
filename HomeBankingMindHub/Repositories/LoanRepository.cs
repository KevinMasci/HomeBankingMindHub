using HomeBankingMindHub.Data;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll().ToList();
        }

        public Loan FindById(long id) 
        { 
            return FindByCondition(loan => loan.Id == id).FirstOrDefault();
        }
    }
}
