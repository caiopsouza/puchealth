using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using puchealth.Models;
using puchealth.Persistence;
using puchealth.Services;

namespace puchealth
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static TokenValidationParameters JwtValidationParameters =>
            new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = IEnv.JwtKey,
                ValidateIssuer = true,
                ValidIssuer = IEnv.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = IEnv.JwtAudience,
                ValidateLifetime = true
            };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers()
                .AddJsonOptions(opts =>
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                );

            services.AddDbContext<Context>(
                options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<User>();
            services.AddScoped<Paciente>();
            services.AddScoped<Profissional>();
            services.AddScoped<Role>();

            services.AddSingleton<IEnv, Env>();

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.AddIdentityCore<Paciente>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.AddIdentityCore<Profissional>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = JwtValidationParameters;
                });

            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetAssembly(typeof(Startup)));

            services.AddMediatR(typeof(Startup));

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo {Title = "PUC Engenharia de Software | Puchealth API", Version = "v1"});

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.<br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br />
                      Example: 'Bearer <token>'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PUC Engenharia de Software | Puchealth API | v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });

            MigrateAndSeed(app).Wait();
        }

        public static async Task MigrateAndSeed(IApplicationBuilder app)
        {
            // Create the database if it doesn't exist and migrate it
            using (var serviceScopeDb = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                await using var context = serviceScopeDb.ServiceProvider.GetRequiredService<Context>();
                await context.Database.MigrateAsync();
            }

            // Create additional data
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                await using var context = serviceScope.ServiceProvider.GetRequiredService<Context>();

                // Procedimento
                if (!await context.Procedimentos.AnyAsync(e => e.Id == IEnv.Procedimento.Id))
                    context.Procedimentos.Add(new Procedimento
                    {
                        Id = IEnv.Procedimento.Id,
                        Name = IEnv.Procedimento.Name,
                        Descricao = IEnv.Procedimento.Descricao,
                        Tipo = IEnv.Procedimento.Tipo
                    });

                // Especialidades
                if (!await context.Especialidades.AnyAsync(e => e.Id == IEnv.Radiologia.Id))
                    context.Especialidades.Add(new Especialidade
                    {
                        Id = IEnv.Radiologia.Id,
                        Name = IEnv.Radiologia.Name,
                        Descricao = IEnv.Radiologia.Descricao
                    });

                // Endereços
                foreach (var endereco in new[] {IEnv.EnderecoEstab1, IEnv.EnderecoEstab2})
                    if (!await context.Enderecos.AnyAsync(e => e.Id == endereco.Id))
                        context.Enderecos.Add(new Endereco
                        {
                            Id = endereco.Id,
                            Rua = endereco.Rua,
                            Numero = endereco.Numero,
                            Bairro = endereco.Bairro,
                            Cidade = endereco.Cidade,
                            Estado = endereco.Estado,
                            CEP = endereco.CEP
                        });

                // Estabelecimentos
                var estabelecimentos = new[]
                    {(IEnv.Estabelecimento1, IEnv.EnderecoEstab1), (IEnv.Estabelecimento2, IEnv.EnderecoEstab2)};

                foreach (var (estabelecimento, endereco) in estabelecimentos)
                    if (!await context.Estabelecimentos.AnyAsync(e => e.Id == estabelecimento.Id))
                        context.Estabelecimentos.Add(new Estabelecimento
                        {
                            Id = estabelecimento.Id,
                            Nome = estabelecimento.Nome,
                            RazaoSocial = estabelecimento.RazaoSocial,
                            Tipo = estabelecimento.Tipo,
                            EnderecoId = endereco.Id
                        });

                await context.SaveChangesAsync();
            }

            // Create roles
            using (var serviceScopeRole = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                using var roleManager = serviceScopeRole.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                if (await roleManager.FindByNameAsync(IEnv.RoleUser) is null)
                    await roleManager.CreateAsync(new Role(new Guid("520edcd6-2a09-4ea7-92e0-5a25d63cffcb"),
                        IEnv.RoleUser));

                if (await roleManager.FindByNameAsync(IEnv.RoleAdmin) is null)
                    await roleManager.CreateAsync(new Role(new Guid("44b23886-c13a-49b4-9680-c0a6fddb3812"),
                        IEnv.RoleAdmin));

                if (await roleManager.FindByNameAsync(IEnv.RoleSuper) is null)
                    await roleManager.CreateAsync(new Role(new Guid("e479e0a4-6a9f-4a4a-928a-6074cbe4be82"),
                        IEnv.RoleSuper));
            }

            // Create admins
            using (var serviceScopeUser = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                using var userManager = serviceScopeUser.ServiceProvider.GetRequiredService<UserManager<User>>();

                // Create a super-admin
                if (userManager.FindByEmailAsync(IEnv.SuperAdminUserView.Email).Result is null)
                {
                    var admin = new User
                    {
                        Id = IEnv.SuperAdminUserView.Id,
                        Name = IEnv.SuperAdminUserView.Name,
                        UserName = IEnv.SuperAdminUserView.Email,
                        Email = IEnv.SuperAdminUserView.Email
                    };
                    await userManager.CreateAsync(admin, "Supersecretpassw000rd!");
                    await userManager.AddToRoleAsync(admin, IEnv.RoleSuper);
                }

                // Create an admin
                if (userManager.FindByEmailAsync(IEnv.AdminUserView.Email).Result is null)
                {
                    var admin = new User
                    {
                        Id = IEnv.AdminUserView.Id,
                        Name = IEnv.AdminUserView.Name,
                        UserName = IEnv.AdminUserView.Email,
                        Email = IEnv.AdminUserView.Email
                    };
                    await userManager.CreateAsync(admin, "Secretpassw000rd!");
                    await userManager.AddToRoleAsync(admin, IEnv.RoleAdmin);
                }
            }

            // Create profissionais
            using (var serviceScopeUser = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                using var profissionalManager =
                    serviceScopeUser.ServiceProvider.GetRequiredService<UserManager<Profissional>>();

                // Create a super-admin
                if (profissionalManager.FindByEmailAsync(IEnv.Radiologista.Email).Result is null)
                {
                    var profissional = new Profissional
                    {
                        Id = IEnv.Radiologista.Id,
                        Name = IEnv.Radiologista.Name,
                        UserName = IEnv.Radiologista.Email,
                        Email = IEnv.Radiologista.Email,
                        Tipo = IEnv.Radiologista.Tipo,
                        EspecialidadeId = IEnv.Radiologista.Especialidade.Id
                    };
                    await profissionalManager.CreateAsync(profissional, "Profissionalpassw000rd!");
                    await profissionalManager.AddToRoleAsync(profissional, IEnv.RoleUser);
                }
            }

            // Create data that depends on users
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                await using var context = serviceScope.ServiceProvider.GetRequiredService<Context>();

                // Procedimentos oferecidos
                foreach (var oferecido in new[] {IEnv.ProcedimentoOfer1, IEnv.ProcedimentoOfer2})
                    if (!await context.ProcedimentosOferecidos.AnyAsync(e => e.Id == oferecido.Id))
                        context.ProcedimentosOferecidos.Add(new ProcedimentoOferecido
                        {
                            Id = oferecido.Id,
                            ProcedimentoId = oferecido.Procedimento.Id,
                            EstabelecimentoId = oferecido.Estabelecimento.Id,
                            ProfissionalId = oferecido.Profissional.Id,
                            Horario = oferecido.Horario,
                            Duracao = new TimeSpan(0, 0, (int) oferecido.Duracao)
                        });

                await context.SaveChangesAsync();
            }
        }
    }
}