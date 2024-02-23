using ClientService.DTO;
using ClientService.Models;

namespace ClientService.Services
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> GetAllClients();
        ClientDTO FindById(long id);
        ClientDTO FindByEmail(string email);
        void Save(Client client);
        void CreateAccountForClient(long clientId);
        Client RegisterNewClient(SignupDTO client);
        IEnumerable<AccountDTO> GetAccountsByCurrentClient(long clientId);
        Card CreateCardForCurrentClient(ClientDTO clientDTO, CardCreateDTO cardDTO);
    }
}
