using System;
using System.Collections.Generic;
using System.Reflection;
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
            services.AddControllers();

            services.AddDbContext<Context>(
                options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<User>();
            services.AddScoped<Role>();

            services.AddSingleton<IEnv, Env>();
            services.AddSingleton<IProductSeed, ProductSeed>();

            services.AddIdentity<User, Role>()
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Puchealth | Bookmark API", Version = "v1"});

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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Puchealth | Bookmarks Api | v1"));
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

            // Create roles
            // Ids are fixed for convenience
            using (var serviceScopeRole = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                using var roleManager = serviceScopeRole.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                if (await roleManager.FindByNameAsync(IEnv.RoleAdmin) is null)
                {
                    await roleManager.CreateAsync(new Role(new Guid("44b23886-c13a-49b4-9680-c0a6fddb3812"),
                        IEnv.RoleAdmin));
                    await roleManager.CreateAsync(
                        new Role(new Guid("180b8ec6-d32e-4b25-93ad-ce45d7567bdd"), IEnv.RoleClient));
                }
            }

            using (var serviceScopeUser = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                using var userManager = serviceScopeUser.ServiceProvider.GetRequiredService<UserManager<User>>();

                if (userManager.FindByEmailAsync(IEnv.AdminUserView.Email).Result is null)
                {
                    var admin = new User
                    {
                        Id = IEnv.AdminUserView.Id,
                        Name = IEnv.AdminUserView.Name,
                        UserName = IEnv.AdminUserView.Email,
                        Email = IEnv.AdminUserView.Email
                    };
                    await userManager.CreateAsync(admin, "Supersecretpassw000rd!");
                    await userManager.AddToRoleAsync(admin, IEnv.RoleAdmin);
                }
            }
        }
    }
}