using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using puchealth.Views.Users;

namespace puchealth.Services
{
    public interface IEnv
    {
        // Jwt info
        public const string JwtAudience = "puchealth";

        public const string JwtIssuer = "puchealth.issuer";

        // Roles
        public const string RoleClient = "client";
        public const string RoleAdmin = "admin";
        public const string RoleAny = "admin,client";

        // Admin has a fixed Id for convenience
        public static readonly UserView AdminUserView = new()
        {
            Id = Guid.Parse("f7f300c9-18f8-4c66-9e7e-55c5ca378dac"),
            Name = "Admin",
            Email = "admin@puchealth.com.br"
        };

        // FIXME: Better error handling if the key is not defined
        public static SymmetricSecurityKey JwtKey
        {
            get
            {
                var keyStr = Environment.GetEnvironmentVariable("jwt_key")!;
                var keyBytes = Encoding.ASCII.GetBytes(keyStr);
                return new SymmetricSecurityKey(keyBytes);
            }
        }

        Guid NewGuid();
    }
}