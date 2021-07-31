using System;
using Microsoft.AspNetCore.Identity;

namespace puchealth.Models
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; } = null!;
    }
}