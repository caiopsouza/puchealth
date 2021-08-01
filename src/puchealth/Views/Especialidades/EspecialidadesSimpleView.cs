using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views.Especialidades
{
    [AutoMap(typeof(Especialidade))]
    public class EspecialidadeSimpleView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;
    }
}