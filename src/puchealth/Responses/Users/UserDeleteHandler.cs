using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using puchealth.Messages.Users;
using puchealth.Models;
using puchealth.Services;

namespace puchealth.Responses.Users
{
    public class UserDeleteHandler : IRequestHandler<UserDelete, bool>
    {
        private readonly IEnv _env;

        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;

        public UserDeleteHandler(IEnv env, IMapper mapper, UserManager<User> userManager)
        {
            _env = env;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<bool> Handle(UserDelete request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user is null) return false;

            await _userManager.DeleteAsync(user);

            return true;
        }
    }
}