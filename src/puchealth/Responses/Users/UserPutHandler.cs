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
    public class UserPutHandler : IRequestHandler<UserPut, OneOf<UserView?, IEnumerable<IdentityError>>>
    {
        private readonly IEnv _env;

        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;

        public UserPutHandler(IEnv env, IMapper mapper, UserManager<User> userManager)
        {
            _env = env;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<OneOf<UserView?, IEnumerable<IdentityError>>> Handle(UserPut request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user is null)
                return (UserView?) null;

            user.Name = request.Name;
            user.UserName = request.Email;
            user.Email = request.Email;

            var userIdentityResult = await _userManager.UpdateAsync(user);

            if (userIdentityResult.Succeeded)
                return _mapper.Map<UserView>(user);
            return userIdentityResult.Errors.ToList();
        }
    }
}