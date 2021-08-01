using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using puchealth.Messages.Users;
using puchealth.Services;
using puchealth.Views.Procedimentos;

namespace puchealth.Controllers
{
    [Route("v1/[controller]")]
    public class ProcedimentoOferecidoController : Controller
    {
        private readonly IMediator _mediator;

        public ProcedimentoOferecidoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = IEnv.AccessAny)]
        [Route("")]
        public async Task<IEnumerable<ProcedimentoOferecidoView>> GetAll(ProcedimentoOferecidoGetAll request)
        {
            return await _mediator.Send(request);
        }
    }
}