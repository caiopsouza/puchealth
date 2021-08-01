using System;

namespace puchealth.Models
{
    public class ProcedimentoOferecido
    {
        public Guid Id { get; init; }

        public Guid ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }

        public Guid ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }
    }
}