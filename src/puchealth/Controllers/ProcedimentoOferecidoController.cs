using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using puchealth.Messages.Users;
using puchealth.Models;
using puchealth.Services;
using puchealth.Views.Procedimentos;
using puchealth.Views.Users;

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

        /*[HttpGet]
        [Authorize(Roles = IEnv.AccessAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(UserView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(Guid id)
        {
            var res = await _mediator.Send(new UserGetById(id));
            return res != null ? Ok(res) : NotFound();
        }

        private BadRequestObjectResult BadRequestFromIdentityErrors(IEnumerable<IdentityError> errors)
        {
            // Username and email are the same. If it errored with "DuplicateEmail" there'll also be
            // a "DuplicateUserName" message. This avoid redundancy.
            errors = errors.Where(error => error.Code != "DuplicateEmail");
            return BadRequest(errors);
        }

        [HttpPost]
        [Authorize(Roles = IEnv.AccessAdmin)]
        [Route("")]
        [ProducesResponseType(typeof(UserView), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] UserPost data)
        {
            return (await _mediator.Send(data))
                .Match<ActionResult>(
                    user => Created($"v1/users/{user.Id}/", user),
                    BadRequestFromIdentityErrors
                );
        }

        [HttpPut]
        [Authorize(Roles = IEnv.AccessAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(UserView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(Guid id, [FromBody] UserPut data)
        {
            data.Id = id;
            return (await _mediator.Send(data))
                .Match<ActionResult>(user => user != null ? Ok(user) : NotFound(), BadRequestFromIdentityErrors);
        }

        [HttpDelete]
        [Authorize(Roles = IEnv.AccessAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var res = await _mediator.Send(new UserDelete(id));
            return res ? NoContent() : NotFound();
        }*/
    }
}