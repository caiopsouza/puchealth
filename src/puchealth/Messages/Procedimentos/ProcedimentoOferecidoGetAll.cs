using System.Collections.Generic;
using MediatR;
using puchealth.Models;
using puchealth.Views.Procedimentos;

namespace puchealth.Messages.Users
{
    public class ProcedimentoOferecidoGetAll : IRequest<IEnumerable<ProcedimentoOferecidoView>>
    {
        public string? Name { get; set; }

        public string? BairroOuCidade { get; set; }

        public ProcedimentoTipo? Tipo { get; set; }
    }
}