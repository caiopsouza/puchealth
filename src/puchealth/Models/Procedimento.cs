using System;

namespace puchealth.Models
{
    public class Procedimento 
    {
        public Guid Id { get; init; }
        
        public string Name { get; init; } = null!;
        
        public string? Descricao { get; init; }
    }
}