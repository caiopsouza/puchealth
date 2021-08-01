using System;
using Microsoft.AspNetCore.Identity;

namespace puchealth.Models
{
    public class Especialidade
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Descricao { get; set; }
    }
}