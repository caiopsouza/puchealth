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
    [AutoMap(typeof(User))]
    public class UserView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }

    [AutoMap(typeof(User), ReverseMap = true)]
    public class UserPostView
    {
        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string Password { get; init; } = null!;
    }

    [AutoMap(typeof(User), ReverseMap = true)]
    public class UserPutView
    {
        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }

    [Route("v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly Context _context;

        private readonly IEnv _env;

        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;

        public UsersController(IEnv env, Context context, IMapper mapper, UserManager<User> userManager)
        {
            _env = env;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("")]
        public IAsyncEnumerable<UserView> GetAll() =>
            (
                from user in _context.Users
                orderby user.Name, user.Id
                select user
            )
            .ProjectTo<UserView>(_mapper.ConfigurationProvider)
            .AsAsyncEnumerable();

        [HttpGet]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(UserView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(Guid id)
        {
            var user = await _context.Users
                .ProjectTo<UserView>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null
                ? NotFound()
                : Ok(user);
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
        [ProducesResponseType(typeof(UserView), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] UserPostView data)
        {
            var user = _mapper.Map<User>(data);

            user.UserName = data.Email;
            user.Id = _env.NewGuid();

            var userIdentityResult = await _userManager.CreateAsync(user, data.Password);

            if (userIdentityResult.Succeeded)
                return Created($"v1/users/{user.Id}/", _mapper.Map<UserView>(user));

            return BadRequestFromIdentityErrors(userIdentityResult);
        }

        [HttpPut]
        [Authorize(Roles = IEnv.RoleAdmin)]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(UserView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(Guid id, [FromBody] UserPutView data)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return NotFound();

            user.Name = data.Name;
            user.UserName = data.Email;
            user.Email = data.Email;

            var userIdentityResult = await _userManager.UpdateAsync(user);

            if (userIdentityResult.Succeeded)
                return Ok(_mapper.Map<UserView>(user));

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
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return NotFound();

            await _userManager.DeleteAsync(user);

            return Ok();
        }
    }
}