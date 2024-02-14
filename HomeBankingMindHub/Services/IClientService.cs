using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> GetAllClients();
        ClientDTO FindById(long id);
        Client FindByEmail(string email);
        void Save(Client client);
    }
}
