using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountService _accountService;
        private readonly ICardRepository _cardRepository;

        public ClientService(IClientRepository clientRepository, IAccountService accountService, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountService = accountService;
            _cardRepository = cardRepository;
        }

        public IEnumerable<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            var clientsDTO = clients.Select(client => new ClientDTO(client));
            return clientsDTO;
        }

        public ClientDTO FindById(long id)
        {
            var client = _clientRepository.FindById(id);

            if (client == null)
            {
                return null;
            }

            var clientDTO = new ClientDTO(client);
            return clientDTO;
        }

        public Client RegisterNewClient(SignupDTO client)
        {
            Client newClient = new Client
            {
                Email = client.Email,
                Password = PasswordHasher.HashPassword(client.Password),
                FirstName = client.FirstName,
                LastName = client.LastName,
            };

            _clientRepository.Save(newClient);

            // Crear automáticamente una cuenta para el nuevo cliente
            CreateAccountForClient(newClient.Id);

            return newClient;
        }

        public void CreateAccountForClient(long clientId)
        {
            var random = new Random();
            string accountNumber;

            // Validar que el número de cuenta no exista previamente
            do
            {
                accountNumber = "VIN-" + random.Next(100, 99999999).ToString();
            } while (_accountService.GetAccountByNumber(accountNumber) != null);

            var account = new Account
            {
                ClientId = clientId,
                Number = accountNumber,
                Balance = 0,
                CreationDate = DateTime.Now,
            };

            _accountService.Save(account);
        }

        public ClientDTO FindByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return null;
            }

            return new ClientDTO(client);
        }

        public IEnumerable<AccountDTO> GetAccountsByCurrentClient(long clientId)
        {
            // Obtener cuentas del cliente
            var accounts = _accountService.GetAccountsByClient(clientId);
            var accountsDTO = accounts.Select(account => new AccountDTO(account));

            return accountsDTO;
        }

        public Card CreateCardForCurrentClient(ClientDTO clientDTO, CardCreateDTO cardDTO)
        {
            // Generar un numero de tarjeta y verificar que no exista
            string cardNumber;

            do
            {
                cardNumber = CardNumberGenerator.GenerateCardNumber();
            } while (_cardRepository.FindByNumber(cardNumber) != null);

            var newCard = new Card
            {
                ClientId = clientDTO.Id,
                CardHolder = clientDTO.FirstName + " " + clientDTO.LastName,
                Type = Enum.Parse<CardType>(cardDTO.Type),
                Color = Enum.Parse<CardColor>(cardDTO.Color),
                Number = cardNumber,
                Cvv = CardNumberGenerator.GenerateCvv(),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(4),
            };

            _cardRepository.Save(newCard);

            return newCard;
        }

        public void Save(Client client)
        {
            _clientRepository.Save(client);
        }
    }
}
