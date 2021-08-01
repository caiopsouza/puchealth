using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using puchealth.Models;
using puchealth.Views;
using puchealth.Views.Especialidades;
using puchealth.Views.Procedimentos;
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

        public static readonly ProfissionalView Radiologista = new()
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
            Id = Radiologista.Id,
            Name = Radiologista.Name,
            Email = Radiologista.Email
        };

        public static readonly EnderecoView EnderecoEstab1 = new()
        {
            Id = new Guid("cc84b3b4-d16e-466b-aecb-a44409b03b26"),
            Rua = "dos Bobos",
            Numero = "0",
            Bairro = "Jardim da Penha",
            Cidade = "Vitória",
            Estado = "ES",
            CEP = "29000-00"
        };

        public static readonly EnderecoView EnderecoEstab2 = new()
        {
            Id = new Guid("b15cea3a-8147-49f7-bfc7-37464f08d7d2"),
            Rua = "Principal",
            Numero = "42",
            Bairro = "Vitória",
            Cidade = "Belo Horizonte",
            Estado = "MG",
            CEP = "30120-000"
        };

        public static readonly EstabelecimentoView Estabelecimento1 = new()
        {
            Id = new Guid("b5f21765-f267-4b7f-9d22-70234f0f8bbd"),
            Nome = "Posto de Saúde JP",
            RazaoSocial = "Unidade Básica de Atendimento de Jardim da Penha",
            Tipo = EstabelecimentoTipo.PostoDeSaude,
            Endereco =
                $"Rua {EnderecoEstab1.Rua}, n. {EnderecoEstab1.Numero}. {EnderecoEstab1.Bairro}, {EnderecoEstab1.Cidade} - {EnderecoEstab1.Estado}"
        };

        public static readonly EstabelecimentoView Estabelecimento2 = new()
        {
            Id = new Guid("337c24f5-704d-4ac5-9985-655e628f3925"),
            Nome = "Clínica BH",
            RazaoSocial = "Clínica Hospitalar Belzonti",
            Tipo = EstabelecimentoTipo.Clinica,
            Endereco =
                $"Rua {EnderecoEstab2.Rua}, n. {EnderecoEstab2.Numero}. {EnderecoEstab2.Bairro}, {EnderecoEstab2.Cidade} - {EnderecoEstab2.Estado}"
        };

        public static readonly ProcedimentoView Procedimento = new()
        {
            Id = new Guid("0177447d-f416-46ba-9f26-f1ce034892b6"),
            Name = "Raio X",
            Descricao = "Raio X do tórax e extremidades excetuando face."
        };

        public static readonly ProcedimentoOferecidoView ProcedimentoOfer1 = new()
        {
            Id = new Guid("4763f84a-f487-42f0-9c2a-82556edbfc43"),
            Procedimento = new ProcedimentoView
            {
                Id = Procedimento.Id,
                Name = Procedimento.Name,
                Descricao = Procedimento.Descricao,
                Tipo = ProcedimentoTipo.Exame
            },
            Estabelecimento = new EstabelecimentoView
            {
                Id = Estabelecimento1.Id,
                Nome = Estabelecimento1.Nome,
                RazaoSocial = Estabelecimento1.RazaoSocial,
                Tipo = Estabelecimento1.Tipo,
                Endereco = Estabelecimento1.Endereco
            },
            Profissional = new ProfissionalView
            {
                Id = Radiologista.Id,
                Name = Radiologista.Name,
                Email = Radiologista.Email,
                Tipo = Radiologista.Tipo,
                Especialidade = new EspecialidadeSimpleView
                {
                    Id = Radiologia.Id,
                    Name = Radiologia.Name
                }
            },
            Horario = new DateTime(2006, 06, 06, 07, 00, 00),
            Duracao = 1800
        };

        public static readonly ProcedimentoOferecidoView ProcedimentoOfer2 = new()
        {
            Id = new Guid("195b536a-1f9a-4da2-921e-69594b255883"),
            Procedimento = new ProcedimentoView
            {
                Id = Procedimento.Id,
                Name = Procedimento.Name,
                Descricao = Procedimento.Descricao
            },
            Estabelecimento = new EstabelecimentoView
            {
                Id = Estabelecimento2.Id,
                Nome = Estabelecimento2.Nome,
                RazaoSocial = Estabelecimento2.RazaoSocial,
                Tipo = Estabelecimento2.Tipo,
                Endereco = Estabelecimento2.Endereco
            },
            Profissional = new ProfissionalView
            {
                Id = Radiologista.Id,
                Name = Radiologista.Name,
                Email = Radiologista.Email,
                Tipo = Radiologista.Tipo,
                Especialidade = new EspecialidadeSimpleView
                {
                    Id = Radiologia.Id,
                    Name = Radiologia.Name
                }
            },
            Horario = new DateTime(2006, 06, 07, 14, 00, 00),
            Duracao = 2700
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