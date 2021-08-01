using System;

namespace puchealth.Models
{
    public class Profissional : User
    {
        public ProfissionalTipo Tipo { get; set; }

        public Guid EspecialidadeId { get; set; }
        public Especialidade Especialidade { get; set; }
    }
}