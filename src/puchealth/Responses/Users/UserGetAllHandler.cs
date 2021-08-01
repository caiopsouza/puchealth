using System.Collections.Generic;
using System.Linq;
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
    public class UserGetAllHandler : IRequestHandler<UserGetAll, IEnumerable<UserView>>
    {
        private readonly Context _context;

        private readonly IMapper _mapper;

        public UserGetAllHandler(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserView>> Handle(UserGetAll request, CancellationToken cancellationToken)
        {
            return await
                (
                    from user in _context.Users
                    orderby user.Name, user.Id
                    select user
                )
                .ProjectTo<UserView>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}