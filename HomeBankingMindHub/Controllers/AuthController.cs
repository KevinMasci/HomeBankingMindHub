using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IClientRepository _clientRepository;

        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                // Verifica si se proporcionaron ambos campos obligatorios
                if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                {
                    return BadRequest("Por favor, proporciona tanto el correo electrónico como la contraseña.");
                }

                Client user = _clientRepository.FindByEmail(login.Email);

                if (user == null || !PasswordHasher.VerifyPassword(login.Password, user.Password))
                {
                    return Unauthorized("Credenciales incorrectas");
                }

                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
