using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using puchealth;
using puchealth.Services;
using Tests.Setup;
using Xunit;

namespace Tests
{
    public class AuthorizationTests : IntegrationTest
    {
        public AuthorizationTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public static TheoryData<string, string, string, bool> PermissionData => new()
        {
            {"account", "login", "post", true},
            {"clients", "", "get", true},
            {"clients", "/00000001-0000-0000-0000-000000000000", "get", true},
            {"clients", "", "post", false},
            {"clients", "/00000001-0000-0000-0000-000000000000", "put", false},
            {"clients", "/00000001-0000-0000-0000-000000000000", "delete", false},
            {"bookmarks", "", "get", true},
            {"bookmarks", "/00000001-0000-0000-0000-000000000000", "post", true},
            {"bookmarks", "/00000001-0000-0000-0000-000000000000", "delete", true},
        };

        [Fact]
        public async Task LoginInfo()
        {
            // Act
            var (response, actual) = await LoginHttpResponse(IEnv.AdminClientView.Email, "Supersecretpassw000rd!");
            response.EnsureSuccessStatusCode();

            var jwtHandler = new JwtSecurityTokenHandler();
            jwtHandler.ValidateToken(actual, Startup.JwtValidationParameters, out var securityToken);
            var token = (JwtSecurityToken) securityToken;

            // Assert
            token.Issuer.Should().BeEquivalentTo(IEnv.JwtIssuer);
            token.Audiences.Should().BeEquivalentTo(IEnv.JwtAudience);

            string GetClaim(string claimType) =>
                token.Claims.First(c => c.Type == claimType).Value;

            // For some reason nameidentifier is mapped into nameid. It seems to work otherwise.
            GetClaim("nameid").Should().BeEquivalentTo(IEnv.AdminClientView.Id.ToString());
            GetClaim("email").Should().BeEquivalentTo(IEnv.AdminClientView.Email);
            GetClaim("unique_name").Should().BeEquivalentTo(IEnv.AdminClientView.Name);
            GetClaim("role").Should().BeEquivalentTo(IEnv.RoleAdmin);
        }

        [Theory]
        [InlineData("admin@puchealth.com.br", "Supersecretpassw000rd!", true)]
        [InlineData("administrator@puchealth.com.br", "Supersecretpassw000rd!", false)] // Wrong user
        [InlineData("admin@puchealth.com.br", "supersecretpassw000rd!", false)] // Wrong password
        [InlineData("administrator@puchealth.com.br", "supersecretpassw000rd!", false)] // Both wrong
        public async Task CanLogin(string username, string password, bool expected)
        {
            // Act
            var (actual, _) = await LoginHttpResponse(username, password);

            // Assert
            actual.IsSuccessStatusCode.Should().Be(expected);
        }

        private async Task<HttpStatusCode> RunVerb(string verb, string url)
        {
            var action = verb switch
            {
                "post" => PostResultAsString(url, null!),
                "put" => PutResultAsString(url, null!),
                "delete" => Delete(url),
                _ => GetResultAsString(url),
            };
            var (res, _) = await action;
            return res.StatusCode;
        }

        [Theory]
        [MemberData(nameof(PermissionData))]
        public async Task HasPermissionAdmin(string controller, string path, string verb, bool _)
        {
            // Act
            var actual = await RunVerb(verb, controller + path);

            // Assert
            actual.Should().NotBe(HttpStatusCode.MethodNotAllowed);
            actual.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Theory]
        [MemberData(nameof(PermissionData))]
        public async Task HasPermissionClient(string controller, string path, string verb, bool allowed)
        {
            // Arrange
            await CreateAlice();
            await LoginAsAlice();

            // Act
            var actual = await RunVerb(verb, controller + path);

            // Assert
            actual.Should().NotBe(HttpStatusCode.MethodNotAllowed);
            if (allowed)
                actual.Should().NotBe(HttpStatusCode.Forbidden);
            else
                actual.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}