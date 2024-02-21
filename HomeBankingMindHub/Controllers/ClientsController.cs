using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;

        public ClientsController(IClientService clientService, IAccountService accountService)
        {
            _clientService = clientService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clientsDTO = _clientService.GetAllClients();
                if (clientsDTO == null)
                {
                    return NotFound("No clients found");
                }
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
                    return NotFound($"Client with ID {id} not found.");
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
                // Verificar que los campos esten completos
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                {
                    return BadRequest("Invalid input");
                }

                // Verificar que el email no este en uso
                var existingUser = _clientService.FindByEmail(client.Email);
                if (existingUser != null)
                {
                    return BadRequest("email already in use");
                }

                Client newClient = _clientService.RegisterNewClient(client);

                return Ok(newClient);
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
                    return NotFound("Client not found.");
                }

                ClientDTO clientDTO = _clientService.FindByEmail(email);

                if (clientDTO == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

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
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("Client not found.");
                }

                var client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

                // Verificar si el cliente ya tiene 3 cuentas registradas
                if (_accountService.GetAccountsByClient(client.Id).Count() >= 3)
                {
                    return BadRequest("The client already has 3 accounts, cannot create more.");
                }

                _clientService.CreateAccountForClient(client.Id);
                return Ok("Account created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetAccountsByCurrentClient()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("Client not found.");
                }

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

                var accountsDTO = _clientService.GetAccountsByCurrentClient(client.Id);
                if (accountsDTO == null)
                {
                    return NotFound("The clients does not have accounts");
                }

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
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("Client not found.");
                }

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

                // Verificar si el cliente ya tiene una tarjeta del mismo tipo y color
                if (client.Cards.Any(c => c.Type == cardDTO.Type && c.Color == cardDTO.Color))
                {
                    return Forbid("Client already has a card with that tipe and color");
                }

                // Verificar si el cliente ya tiene 3 tarjetas del tipo seleccionado
                if (client.Cards.Count(c => c.Type == cardDTO.Type) >= 3)
                {
                    return Forbid("Client already has 3 cards of that type");
                }

                _clientService.CreateCardForCurrentClient(client, cardDTO);

                return Ok("Card created successfully.");
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
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound("Client not found.");
                }

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
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
