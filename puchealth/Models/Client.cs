using System;
using Microsoft.AspNetCore.Identity;

namespace puchealth.Models
{
    public class Client : IdentityUser<Guid>
    {
        public string Name { get; set; } = null!;
    }
}