using System;
using AutoMapper;
using puchealth.Models;
using puchealth.Views.Especialidades;

namespace puchealth.Views.Users
{
    [AutoMap(typeof(Profissional))]
    public class ProfissionalView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;

        public ProfissionalTipo Tipo { get; set; }

        public EspecialidadeSimpleView Especialidade { get; set; }
    }
}