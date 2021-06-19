using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using puchealth.Models;
using puchealth.Persistence;
using puchealth.Services;

namespace puchealth.Controllers
{
    [AutoMap(typeof(Client))]
    public class ClientView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }

    [AutoMap(typeof(Client), ReverseMap = true)]
    public class ClientPostView
    {
        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string Password { get; init; } = null!;
    }

    [AutoMap(typeof(Client), ReverseMap = true)]
    public class ClientPutView
    {
        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }

    [Route("v1/[controller]")]
    public class ClientsController : Controller
    {
        private readonly Context _context;

        private readonly IEnv _env;

        private readonly IMapper _mapper;

        private readonly UserManager<Client> _userManager;

        public ClientsController(IEnv env, Context context, IMapper mapper, UserManager<Client> userManager)
        {
            _env = env;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("")]
        public IAsyncEnumerable<ClientView> GetAll() =>
            (
                from client in _context.Users
                orderby client.Name, client.Id
                select client
            )
            .ProjectTo<ClientView>(_mapper.ConfigurationProvider)
            .AsAsyncEnumerable();

        [HttpGet]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(ClientView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(Guid id)
        {
            var client = await _context.Users
                .ProjectTo<ClientView>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == id);

            return client == null
                ? NotFound()
                : Ok(client);
        }

        private BadRequestObjectResult BadRequestFromIdentityErrors(IdentityResult result)
        {
            // Username and email are the same. If it errored with "DuplicateEmail" there'll also be
            // a "DuplicateUserName" message. This avoid redundancy.
            var errors = result.Errors
                .Where(error => error.Code != "DuplicateEmail");

            return BadRequest(errors);
        }

        [HttpPost]
        [Authorize(Roles = IEnv.RoleAdmin)]
        [Route("")]
        [ProducesResponseType(typeof(ClientView), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] ClientPostView data)
        {
            var client = _mapper.Map<Client>(data);

            client.UserName = data.Email;
            client.Id = _env.NewGuid();

            var userIdentityResult = await _userManager.CreateAsync(client, data.Password);

            if (userIdentityResult.Succeeded)
                return Created($"v1/clients/{client.Id}/", _mapper.Map<ClientView>(client));

            return BadRequestFromIdentityErrors(userIdentityResult);
        }

        [HttpPut]
        [Authorize(Roles = IEnv.RoleAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(ClientView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(Guid id, [FromBody] ClientPutView data)
        {
            var client = await _userManager.FindByIdAsync(id.ToString());

            if (client is null)
                return NotFound();

            client.Name = data.Name;
            client.UserName = data.Email;
            client.Email = data.Email;

            var userIdentityResult = await _userManager.UpdateAsync(client);

            if (userIdentityResult.Succeeded)
                return Ok(_mapper.Map<ClientView>(client));

            return BadRequestFromIdentityErrors(userIdentityResult);
        }

        // FIXME: Test if the user is deleted when they have bookmarks
        [HttpDelete]
        [Authorize(Roles = IEnv.RoleAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var client = await _userManager.FindByIdAsync(id.ToString());

            if (client is null)
                return NotFound();

            await _userManager.DeleteAsync(client);

            return Ok();
        }
    }
}