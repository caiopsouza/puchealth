using System;

namespace puchealth.Models
{
    public class ProcedimentoOferecido
    {
        public Guid Id { get; init; }

        public Guid ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }

        public Guid EstabelecimentoId { get; set; }
        public Estabelecimento Estabelecimento { get; set; }

        public Guid ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }

        public DateTime Horario { get; set; }

        public TimeSpan Duracao { get; set; }
    }
}