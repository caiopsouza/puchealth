using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views.Procedimentos
{
    [AutoMap(typeof(Procedimento))]
    public class ProcedimentoView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string? Descricao { get; init; }

        public ProcedimentoTipo Tipo { get; set; }
    }
}