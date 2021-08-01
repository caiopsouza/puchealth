using System.Collections.Generic;
using MediatR;
using puchealth.Views.Procedimentos;

namespace puchealth.Messages.Users
{
    public class ProcedimentoOferecidoGetAll : IRequest<IEnumerable<ProcedimentoOferecidoView>>
    {
    }
}