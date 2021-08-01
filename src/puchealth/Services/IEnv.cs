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
        public const string RoleUser = "user";
        public const string RoleAdmin = "admin";
        public const string RoleSuper = "super";

        // Access to
        public const string AccessUser = "user,admin,super";
        public const string AccessAdmin = "admin,super";
        public const string AccessSuper = "super";

        // Admins have a fixed Id for convenience
        public static readonly UserView SuperAdminUserView = new()
        {
            Id = Guid.Parse("f7f300c9-18f8-4c66-9e7e-55c5ca378dac"),
            Name = "SuperAdmin",
            Email = "superadmin@puchealth.com.br"
        };

        // Admins have a fixed Id for convenience
        public static readonly UserView AdminUserView = new()
        {
            Id = Guid.Parse("a5de6903-e941-44d5-8644-5d390a0d0761"),
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