using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        IEnumerable<Client> GetAllClients();
        Client FindById(long id);
        Client FindByEmail(string email);
        void Save(Client client);
    }
}
