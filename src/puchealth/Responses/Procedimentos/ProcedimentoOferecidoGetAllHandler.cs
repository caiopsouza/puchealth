using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using puchealth.Messages.Users;
using puchealth.Persistence;
using puchealth.Views;
using puchealth.Views.Especialidades;
using puchealth.Views.Procedimentos;
using puchealth.Views.Users;

namespace puchealth.Responses.Users
{
    public class
        ProcedimentoOferecidoGetAllHandler : IRequestHandler<ProcedimentoOferecidoGetAll,
            IEnumerable<ProcedimentoOferecidoView>>
    {
        private readonly Context _context;

        private readonly IMapper _mapper;

        public ProcedimentoOferecidoGetAllHandler(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProcedimentoOferecidoView>> Handle(ProcedimentoOferecidoGetAll request,
            CancellationToken cancellationToken)
        {
            return await
                (
                    from oferecido in _context.ProcedimentosOferecidos
                    let procedimento = oferecido.Procedimento
                    let estabelecimento = oferecido.Estabelecimento
                    let endereco = estabelecimento.Endereco
                    let profissional = oferecido.Profissional
                    let especialidade = profissional.Especialidade
                    where
                        (
                            request.Name == null
                            || procedimento.Name.Contains(request.Name)
                        )
                        && (
                            request.BairroOuCidade == null
                            || endereco.Bairro.Contains(request.BairroOuCidade)
                            || endereco.Cidade.Contains(request.BairroOuCidade)
                        )
                        && (
                            request.Tipo == null
                            || procedimento.Tipo == request.Tipo
                        )
                    orderby
                        procedimento.Name,
                        profissional.Name,
                        estabelecimento.Nome,
                        estabelecimento.Endereco,
                        oferecido.Id
                    select new ProcedimentoOferecidoView
                    {
                        Id = oferecido.Id,
                        Procedimento = new ProcedimentoView
                        {
                            Id = procedimento.Id,
                            Name = procedimento.Name,
                            Descricao = procedimento.Descricao,
                            Tipo = procedimento.Tipo
                        },
                        Estabelecimento = new EstabelecimentoView
                        {
                            Id = estabelecimento.Id,
                            Nome = estabelecimento.Nome,
                            RazaoSocial = estabelecimento.RazaoSocial,
                            Tipo = estabelecimento.Tipo,
                            Endereco =
                                $"Rua {endereco.Rua}, n. {endereco.Numero}. {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}"
                        },
                        Profissional = new ProfissionalView
                        {
                            Id = profissional.Id,
                            Name = profissional.Name,
                            Email = profissional.Email,
                            Tipo = profissional.Tipo,
                            Especialidade = new EspecialidadeSimpleView
                            {
                                Id = especialidade.Id,
                                Name = especialidade.Name
                            }
                        },
                        Horario = oferecido.Horario,
                        Duracao = oferecido.Duracao.TotalSeconds
                    }
                )
                .ToListAsync(cancellationToken);
        }
    }
}