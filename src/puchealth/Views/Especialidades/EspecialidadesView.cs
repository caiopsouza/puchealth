using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views.Especialidades
{
    [AutoMap(typeof(Especialidade))]
    public class EspecialidadeView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string? Descricao { get; init; } = null!;
    }
}