using System;
using Microsoft.AspNetCore.Identity;

namespace puchealth.Models
{
    public sealed class Role : IdentityRole<Guid>
    {
        public Role()
        {
        }

        public Role(Guid id, string name) : base(name)
        {
            Id = id;
        }
    }
}