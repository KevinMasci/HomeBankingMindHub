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
                Client newClient = _clientService.RegisterNewClient(client);

                return Created("", newClient);
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
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
                }

                ClientDTO clientDTO = _clientService.FindByEmail(email);

                if (clientDTO == null)
                {
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
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
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
                }

                _clientService.CreateAccountForClient(client.Id);
                return Ok();
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
                // Obtener el cliente actual
                var email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("No se ha encontrado el cliente asociado al usuario actual.");
                }

                var accountsDTO = _clientService.GetAccountsByCurrentClient(client.Id);

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

                ClientDTO client = _clientService.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("Client not found.");
                }

                // Obtener el tipo y el color de la nueva tarjeta
                var cardType = cardDTO.Type;
                var cardColor = cardDTO.Color;

                // Verificar si el cliente ya tiene una tarjeta del mismo tipo y color
                if (client.Cards.Any(c => c.Type == cardType && c.Color == cardColor))
                {
                    return StatusCode(403, $"El cliente ya tiene una tarjeta de tipo {cardType} y color {cardColor}");
                }

                // Verificar si el cliente ya tiene 3 tarjetas del tipo seleccionado
                if (client.Cards.Count(c => c.Type == cardType) >= 3)
                {
                    return StatusCode(403, $"El cliente ya tiene 3 tarjetas de tipo {cardType}");
                }

                _clientService.CreateCardForCurrentClient(client, cardDTO);

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

                ClientDTO client = _clientService.FindByEmail(email);

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
