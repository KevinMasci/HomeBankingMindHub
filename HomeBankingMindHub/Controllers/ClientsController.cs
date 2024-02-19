using HomeBankingMindHub.DTO;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
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
                Client newClient = _clientService.RegisterNewClient(client);

                return Ok(newClient);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
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

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("The client associated with the current user has not been found.");
                }

                _clientService.CreateAccountForClient(client.Id);
                return Ok("Account created successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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

                _clientService.CreateCardForCurrentClient(client, cardDTO);

                return StatusCode(201, "Card created successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
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
