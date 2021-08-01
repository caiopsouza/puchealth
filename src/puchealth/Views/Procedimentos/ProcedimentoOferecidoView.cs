using System;
using puchealth.Views.Users;

namespace puchealth.Views.Procedimentos
{
    public class ProcedimentoOferecidoView
    {
        public Guid Id { get; init; }

        public ProcedimentoView Procedimento { get; set; }

        public EstabelecimentoView Estabelecimento { get; set; }

        public ProfissionalView Profissional { get; set; }

        public DateTime Horario { get; set; }

        public double Duracao { get; set; }
    }
}