using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneOf;
using puchealth.Messages.Users;
using puchealth.Models;
using puchealth.Services;
using puchealth.Views.Users;

namespace puchealth.Responses.Users
{
    public class UserPostHandler : IRequestHandler<UserPost, OneOf<UserView, IEnumerable<IdentityError>>>
    {
        private readonly IEnv _env;

        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;

        public UserPostHandler(IEnv env, IMapper mapper, UserManager<User> userManager)
        {
            _env = env;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<OneOf<UserView, IEnumerable<IdentityError>>> Handle(UserPost request,
            CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);

            user.UserName = request.Email;
            user.Id = _env.NewGuid();

            var userIdentityResult = await _userManager.CreateAsync(user, request.Password);

            if (userIdentityResult.Succeeded)
                return _mapper.Map<UserView>(user);
            return userIdentityResult.Errors.ToList();
        }
    }
}