using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views.Procedimentos
{
    [AutoMap(typeof(Procedimento))]
    public class ProcedimentoOferecidoView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }
}