using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using puchealth.Messages.Account;
using puchealth.Models;
using puchealth.Services;

namespace puchealth.Responses.Account
{
    public class UserLoginHandler : IRequestHandler<AccountLogin, string?>
    {
        private readonly UserManager<User> _userManager;

        public UserLoginHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> Handle(AccountLogin request, CancellationToken cancellationToken)
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);
            if (userIdentity is null || !await _userManager.CheckPasswordAsync(userIdentity, request.Password))
                return null;

            var role = await _userManager.IsInRoleAsync(userIdentity, IEnv.RoleAdmin)
                ? IEnv.RoleAdmin
                : IEnv.RoleUser;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, userIdentity.Name),
                    new(ClaimTypes.NameIdentifier, userIdentity.Id.ToString()),
                    new(ClaimTypes.Email, userIdentity.Email),
                    new(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(IEnv.JwtKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = IEnv.JwtIssuer,
                Audience = IEnv.JwtAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}