using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using puchealth.Models;
using puchealth.Services;

namespace puchealth.Controllers
{
    public class ClientLogin
    {
        public string Email { get; init; } = default!;

        public string Password { get; init; } = default!;
    }

    [Route("v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<Client> _userManager;

        public AccountController(UserManager<Client> userManager) => _userManager = userManager;

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<string>> Authenticate([FromBody] ClientLogin client)
        {
            var userIdentity = await _userManager.FindByEmailAsync(client.Email);
            if (userIdentity is null || !await _userManager.CheckPasswordAsync(userIdentity, client.Password))
                return Unauthorized();

            var role = await _userManager.IsInRoleAsync(userIdentity, IEnv.RoleAdmin)
                ? IEnv.RoleAdmin
                : IEnv.RoleClient;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, userIdentity.Name),
                    new(ClaimTypes.NameIdentifier, userIdentity.Id.ToString()),
                    new(ClaimTypes.Email, userIdentity.Email),
                    new(ClaimTypes.Role, role),
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(IEnv.JwtKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = IEnv.JwtIssuer,
                Audience = IEnv.JwtAudience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}