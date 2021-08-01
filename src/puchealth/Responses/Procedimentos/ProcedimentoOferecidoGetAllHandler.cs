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
using puchealth.Views.Procedimentos;
using puchealth.Views.Users;

namespace puchealth.Responses.Users
{
    public class ProcedimentoOferecidoGetAllHandler : IRequestHandler<ProcedimentoOferecidoGetAll, IEnumerable<ProcedimentoOferecidoView>>
    {
        private readonly Context _context;

        private readonly IMapper _mapper;

        public ProcedimentoOferecidoGetAllHandler(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProcedimentoOferecidoView>> Handle(ProcedimentoOferecidoGetAll request, CancellationToken cancellationToken)
        {
            return await
                (
                    from proc in _context.Procedimentos
                    orderby proc.Name, proc.Id
                    select proc
                )
                .ProjectTo<ProcedimentoOferecidoView>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}