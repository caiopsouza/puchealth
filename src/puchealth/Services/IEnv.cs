using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using puchealth.Models;
using puchealth.Views.Especialidades;
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
        public const string AccessAny = "user,admin,super";
        public const string AccessAdmin = "admin,super";
        public const string AccessSuper = "super";

        // Data have a fixed Id for convenience
        public static readonly UserView SuperAdminUserView = new()
        {
            Id = Guid.Parse("f7f300c9-18f8-4c66-9e7e-55c5ca378dac"),
            Name = "SuperAdmin",
            Email = "superadmin@puchealth.com.br"
        };

        public static readonly UserView AdminUserView = new()
        {
            Id = Guid.Parse("a5de6903-e941-44d5-8644-5d390a0d0761"),
            Name = "Admin",
            Email = "admin@puchealth.com.br"
        };

        public static readonly EspecialidadeView Radiologia = new()
        {
            Id = new Guid("5d7a578c-e69c-4b37-bad9-67a2b54da1a7"),
            Name = "Radiologia",
            Descricao = "Especialidade em exames Raio X."
        };

        public static readonly ProfissionalView ProfissionalView = new()
        {
            Id = Guid.Parse("7eac7b1b-cd5e-4faa-817e-d48f6e41e5dc"),
            Name = "Rafael Radio",
            Email = "rafael.radio@puchealth.com.br",
            Tipo = ProfissionalTipo.Medico,
            Especialidade = new EspecialidadeSimpleView
            {
                Id = Radiologia.Id,
                Name = Radiologia.Name
            }
        };

        public static readonly UserView ProfissionalUserView = new()
        {
            Id = ProfissionalView.Id,
            Name = ProfissionalView.Name,
            Email = ProfissionalView.Email,
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