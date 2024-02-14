using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ICardRepository _cardRepository;

        public ClientsController(IClientService clientService, IAccountService accountService, ICardRepository cardRepository)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clientsDTO = _clientService.GetAllClients();
                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var clientDTO = _clientService.FindById(id);

                if (clientDTO == null)
                {
                    return Forbid();
                }

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] SignupDTO client)
        {
            try
            {
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                Client user = _clientService.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = PasswordHasher.HashPassword(client.Password),
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientService.Save(newClient);
                // Crear automáticamente una cuenta para el nuevo cliente
                CreateAccountForClient(newClient.Id);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
                }

                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
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
                        Type = c.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        public IActionResult CreateAccountForCurrentClient()
        {
            try
            {
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verificar si el cliente ya tiene 3 cuentas registradas
                if (_accountService.GetAccountsByClient(client.Id).Count() >= 3)
                {
                    return StatusCode(403, "El cliente ya tiene 3 cuentas registradas, no se puede crear más.");
                }

                CreateAccountForClient(client.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Metodo para crear una cuenta a un usuario dado su Id
        private void CreateAccountForClient(long clientId)
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

        [HttpGet("current/accounts")]
        public IActionResult GetAccountsByCurrentClient()
        {
            try
            {
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Obtener cuentas del cliente
                var accounts = _accountService.GetAccountsByClient(client.Id);
                var accountsDTO = accounts.Select(account => new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(tr => new TransactionDTO
                    {
                        Id = tr.Id,
                        Type = tr.Type.ToString(),
                        Amount = tr.Amount,
                        Description = tr.Description,
                        Date = tr.Date,
                    }).ToList()
                }).ToList();

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        public IActionResult CreateCardForCurrentClient([FromBody] CardCreateDTO cardDTO)
        {
            try
            {
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verificar si el cliente ya tiene 3 tarjetas del tipo seleccionado
                var cardType = Enum.Parse<CardType>(cardDTO.Type);
                if (client.Cards.Count(c => c.Type == cardType) >= 3)
                {
                    return StatusCode(403, "El cliente ya tiene 3 tarjetas de ese tipo");
                }

                // Generar un numero de tarjeta y verificar que no exista
                string cardNumber;

                do
                {
                    cardNumber = CardNumberGenerator.GenerateCardNumber();
                } while (_cardRepository.FindByNumber(cardNumber) != null);

                var newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = Enum.Parse<CardType>(cardDTO.Type),
                    Color = Enum.Parse<CardColor>(cardDTO.Color),
                    Number = cardNumber,
                    Cvv = CardNumberGenerator.GenerateCvv(),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(4),
                };

                _cardRepository.Save(newCard);

                return StatusCode(201, "Card created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetCardsForCurrentClient()
        {
            try
            {
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                return Ok(client.Cards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
