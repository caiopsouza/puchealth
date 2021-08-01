using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using puchealth.Messages.Users;
using puchealth.Persistence;
using puchealth.Views.Users;

namespace puchealth.Responses.Users
{
    public class UserGetByIdHandler : IRequestHandler<UserGetById, UserView?>
    {
        private readonly Context _context;

        private readonly IMapper _mapper;

        public UserGetByIdHandler(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserView?> Handle(UserGetById request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .ProjectTo<UserView>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        }
    }
}