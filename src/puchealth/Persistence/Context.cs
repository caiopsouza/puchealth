using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using puchealth.Models;

namespace puchealth.Persistence
{
    public class Context : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Especialidade> Especialidades { get; init; } = null!;

        public DbSet<Profissional> Profissionais { get; init; } = null!;

        public DbSet<Paciente> Pacientes { get; init; } = null!;

        public DbSet<Procedimento> Procedimentos { get; init; } = null!;

#pragma warning disable 8618 // They are automagically initialized by the framework
        public Context(DbContextOptions<Context> options) : base(options)
#pragma warning restore 8618
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.LogTo(Console.WriteLine);
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        }
    }
}