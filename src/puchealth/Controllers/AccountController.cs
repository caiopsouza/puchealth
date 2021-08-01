using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using puchealth.Messages.Account;

namespace puchealth.Controllers
{
    [Route("v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<string>> Authenticate([FromBody] AccountLogin account)
        {
            var res = await _mediator.Send<string?>(account);
            return res != null ? Ok(res) : Unauthorized();
        }
    }
}