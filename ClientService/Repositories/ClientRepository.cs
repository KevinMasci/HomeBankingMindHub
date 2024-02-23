using ClientService.Data;
using ClientService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Repositories
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IQueryable<Client> IncludeMethod(IQueryable<Client> query)
        {
            return query.Include(client => client.Accounts)
                        .Include(client => client.ClientLoans)
                            .ThenInclude(cl => cl.Loan)
                        .Include(client => client.Cards);
        }

        public Client FindById(long id)
        {
            return IncludeMethod(FindByCondition(client => client.Id == id))
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return IncludeMethod(FindAll())
                .ToList();
        }

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }

        public Client FindByEmail(string email)
        {
            return IncludeMethod(FindByCondition(client => client.Email.ToUpper() == email.ToUpper()))
                .FirstOrDefault();
        }

    }
}
