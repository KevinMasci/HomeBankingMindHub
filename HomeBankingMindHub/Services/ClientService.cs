using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public IEnumerable<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            var clientsDTO = new List<ClientDTO>();

            foreach (Client client in clients)
            {
                var newClientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                clientsDTO.Add(newClientDTO);
            }
            return clientsDTO;
        }

        public ClientDTO FindById(long id)
        {
            var client = _clientRepository.FindById(id);

            if (client == null)
            {
                return null;
            }

            var clientDTO = new ClientDTO
            {
                Id = client.Id,
                Email = client.Email,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Accounts = client.Accounts.Select(ac => new AccountDTO
                {
                    Id = ac.Id,
                    Balance = ac.Balance,
                    CreationDate = ac.CreationDate,
                    Number = ac.Number
                }).ToList(),
                Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                {
                    Id = cl.Id,
                    LoanId = cl.LoanId,
                    Name = cl.Loan.Name,
                    Amount = cl.Amount,
                    Payments = int.Parse(cl.Payments)
                }).ToList(),
                Cards = client.Cards.Select(c => new CardDTO
                {
                    Id = c.Id,
                    CardHolder = c.CardHolder,
                    Color = c.Color.ToString(),
                    Cvv = c.Cvv,
                    FromDate = c.FromDate,
                    Number = c.Number,
                    ThruDate = c.ThruDate,
                    Type = c.Type.ToString(),
                }).ToList()
            };
            return clientDTO;
        }

        public Client RegisterNewClient(SignupDTO clientDTO)
        {
            if (String.IsNullOrEmpty(clientDTO.Email) || String.IsNullOrEmpty(clientDTO.Password) || String.IsNullOrEmpty(clientDTO.FirstName) || String.IsNullOrEmpty(clientDTO.LastName))
            {
                throw new ArgumentException("Datos inválidos");
            }

            Client existingUser = _clientRepository.FindByEmail(clientDTO.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está en uso");
            }

            Client newClient = new Client
            {
                Email = clientDTO.Email,
                Password = PasswordHasher.HashPassword(clientDTO.Password),
                FirstName = clientDTO.FirstName,
                LastName = clientDTO.LastName,
            };

            _clientRepository.Save(newClient);

            // Crear automáticamente una cuenta para el nuevo cliente
            _accountRepository.CreateAccountForClient(newClient.Id);

            return newClient;
        }

        public Client FindByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public void Save(Client client)
        {
            _clientRepository.Save(client);
        }
    }
}
