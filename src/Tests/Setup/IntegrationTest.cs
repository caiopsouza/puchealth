using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using puchealth;
using puchealth.Controllers;
using puchealth.Persistence;
using puchealth.Services;
using Xunit;

namespace Tests.Setup
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private static readonly JsonSerializerOptions JsonDeserializeOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _client;

        private readonly SqliteConnection _connection;

        static IntegrationTest()
        {
            Environment.SetEnvironmentVariable("jwt_key", "JXdB3n8Mz7Jl6jthJxNkt93CvIxeHqJZic/6MOf/rHE=");
        }

        // Used to setup the database for a test
        protected async Task Database(Func<Context, Task> action)
        {
            var options = new DbContextOptionsBuilder<Context>().UseSqlite(_connection);
            await using var context = new Context(options.Options);
            await action(context);
        }

        protected async Task<(HttpResponseMessage, string)> GetResultAsString(string url)
        {
            var res = await _client.GetAsync($"v1/{url}/");
            return (res, await res.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpResponseMessage, TResult)> Get<TResult>(string url)
        {
            var res = await _client.GetAsync($"v1/{url}/");
            var message = await res.Content.ReadFromJsonAsync<TResult>(JsonDeserializeOptions);
            return (res, message!);
        }

        protected async Task<(HttpResponseMessage, string)> PostResultAsString(string url, object data)
        {
            var res = await _client.PostAsJsonAsync($"v1/{url}/", data);
            return (res, await res.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpResponseMessage, TResult)> Post<TResult>(string url, object data)
        {
            var res = await _client.PostAsJsonAsync($"v1/{url}/", data);
            var message = await res.Content.ReadFromJsonAsync<TResult>(JsonDeserializeOptions);
            return (res, message!);
        }

        protected async Task<(HttpResponseMessage, TResult)> Post<TResult>(string url)
        {
            return await Post<TResult>(url, null!);
        }

        protected async Task<(HttpResponseMessage, string)> PutResultAsString(string url, object data)
        {
            var res = await _client.PutAsJsonAsync($"v1/{url}/", data);
            return (res, await res.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpResponseMessage, TResult)> Put<TResult>(string url, object data)
        {
            var res = await _client.PutAsJsonAsync($"v1/{url}/", data);
            var message = await res.Content.ReadFromJsonAsync<TResult>(JsonDeserializeOptions);
            return (res, message!);
        }

        protected async Task<(HttpResponseMessage, string)> Delete(string url)
        {
            var res = await _client.DeleteAsync($"v1/{url}/");
            return (res, await res.Content.ReadAsStringAsync());
        }

        #region Ancillary methods for accounts and login

        protected IntegrationTest(WebApplicationFactory<Startup> factory, string? databasePath = null)
        {
            // Always use the same connection. If the `databasePath` is not informed, create a database in memory.
            var database = databasePath ?? ":memory:";
            _connection = new SqliteConnection($"DataSource={database}");
            _connection.Open();

            var actualFactory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // Remove services that will be mocked
                    var servicesToRemove = new[]
                    {
                        typeof(DbContextOptions<Context>),
                        typeof(IEnv),
                        typeof(IProductSeed)
                    };

                    foreach (var service in servicesToRemove)
                        services.RemoveAll(service);

                    // Replace with an SQLite one for speed and isolation
                    services.AddDbContext<Context>(
                        options => options.UseSqlite(_connection)
                    );

                    // Replace with the mocked environment
                    services.AddSingleton<IEnv, MockedEnv>();

                    // Mocked seed
                    services.AddScoped<IProductSeed, ProductSeed>();
                })
            );

            _client = actualFactory.CreateClient();

            LoginAsAdmin().Wait();
        }

        // Login as the user
        protected async Task<(HttpResponseMessage, string)> LoginHttpResponse(string email, string password)
        {
            var loginData = new UserLogin
            {
                Email = email,
                Password = password
            };

            return await PostResultAsString("account/login", loginData);
        }

        // Login as the user
        private async Task Login(string email, string password)
        {
            var (res, token) = await LoginHttpResponse(email, password);

            res.EnsureSuccessStatusCode();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected readonly UserPostView _alice = new()
        {
            Email = "alice@other.company.com",
            Name = "Alice",
            Password = "AliceSecretPassw000rd!"
        };

        private readonly UserPostView _bob = new()
        {
            Email = "bob@other.company.com",
            Name = "Bob",
            Password = "BobSecretPassw000rd!"
        };

        protected async Task<UserView> CreateAlice()
        {
            return (await Post<UserView>("users", _alice)).Item2;
        }

        protected async Task<UserView> CreateBob()
        {
            return (await Post<UserView>("users", _bob)).Item2;
        }

        protected async Task LoginAsAdmin()
        {
            await Login(IEnv.AdminUserView.Email, "Supersecretpassw000rd!");
        }

        protected async Task LoginAsAlice()
        {
            await Login(_alice.Email, _alice.Password);
        }

        protected async Task LoginAsBob()
        {
            await Login(_bob.Email, _bob.Password);
        }

        #endregion
    }
}