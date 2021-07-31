using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using puchealth;
using puchealth.Controllers;
using puchealth.Services;
using Tests.Setup;
using Xunit;

namespace Tests
{
    public class ClientTests : IntegrationTest
    {
        public ClientTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public static TheoryData<ClientPostView, IEnumerable<IdentityError>> AccountInfoErrorData =>
            new()
            {
                {
                    new ClientPostView
                    {
                        Name = "Caio Souza",
                        Email = "admin@puchealth.com.br",
                        Password = "Passsssssw0rd!",
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "DuplicateUserName",
                            Description = "Username 'admin@puchealth.com.br' is already taken.",
                        },
                    }
                },

                {
                    new ClientPostView
                    {
                        Name = "Caio Souza",
                        Email = "caio.souza.puchealth.com.br",
                        Password = "Passsssssw0rd!",
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "InvalidEmail", Description = "Email 'caio.souza.puchealth.com.br' is invalid.",
                        },
                    }
                },
            };

        public static TheoryData<ClientPostView, IEnumerable<IdentityError>> PasswordErrorData =>
            new()
            {
                {
                    new ClientPostView
                    {
                        Name = "Caio Souza",
                        Email = "caio.souza@puchealth.com.br",
                        Password = "word",
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "PasswordTooShort",
                            Description = "Passwords must be at least 8 characters.",
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresNonAlphanumeric",
                            Description = "Passwords must have at least one non alphanumeric character.",
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresDigit",
                            Description = "Passwords must have at least one digit ('0'-'9').",
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresUpper",
                            Description = "Passwords must have at least one uppercase ('A'-'Z').",
                        },
                    }
                },
            };

        [Fact]
        public async Task GetClients()
        {
            // Act
            var (res, actual) = await Get<List<ClientView>>("clients");

            // Assert
            res.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(IEnv.AdminClientView);
        }

        [Theory]
        [MemberData(nameof(AccountInfoErrorData))]
        [MemberData(nameof(PasswordErrorData))]
        public async Task PostClientError(ClientPostView data, IEnumerable<IdentityError> expected)
        {
            // Act
            var (response, actual) = await Post<List<IdentityError>>("clients", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.Should().BeEquivalentTo(expected, options => options.WithoutStrictOrdering());
        }

        #region Post

        [Fact]
        public async Task PostClient()
        {
            // Arrange
            var data = new ClientPostView
            {
                Email = "caio.souza@puchealth.com.br",
                Name = "Caio Souza",
                Password = "Passw0000000rd!",
            };

            // Act
            var (response, actual) = await Post<ClientView>("clients", data);

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new ClientView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = data.Email,
                Name = data.Name,
            });
        }

        [Fact]
        public async Task GetClientAfterPost()
        {
            // Arrange
            await PostClient();

            // Act
            var (response, actual) = await Get<ClientView>($"clients/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new ClientView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = "caio.souza@puchealth.com.br",
                Name = "Caio Souza",
            });
        }

        [Fact]
        public async Task GetClientListAfterPost()
        {
            // Arrange
            await PostClient();

            // Act
            var (response, actual) = await Get<List<ClientView>>("clients");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new[]
                {
                    IEnv.AdminClientView,
                    new ClientView
                    {
                        Id = MockedEnv.CreateGuid(1),
                        Email = "caio.souza@puchealth.com.br",
                        Name = "Caio Souza",
                    },
                }, options => options.WithStrictOrdering()
            );
        }

        #endregion

        #region Put

        [Theory]
        [MemberData(nameof(AccountInfoErrorData))]
        public async Task PutClientError(ClientPostView data, IEnumerable<IdentityError> expected)
        {
            // Arrange
            await PostClient();

            // Act
            var (response, actual) = await Put<List<IdentityError>>($"clients/{MockedEnv.CreateGuid(1)}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.Should().BeEquivalentTo(expected, options => options.WithoutStrictOrdering());
        }

        [Fact]
        public async Task PutClient()
        {
            // Arrange
            await PostClient();

            var data = new ClientPutView
            {
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza",
            };

            // Act
            var (response, actual) = await Put<ClientView>($"clients/{MockedEnv.CreateGuid(1)}", data);

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new ClientView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = data.Email,
                Name = data.Name,
            });
        }

        [Fact]
        public async Task GetClientAfterPut()
        {
            // Arrange
            await PutClient();

            // Act
            var (response, actual) = await Get<ClientView>($"clients/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new ClientView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza",
            });
        }

        [Fact]
        public async Task GetClientListAfterPut()
        {
            // Arrange
            await PutClient();

            // Act
            var (response, actual) = await Get<List<ClientView>>("clients");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new[]
                {
                    IEnv.AdminClientView,
                    new ClientView
                    {
                        Id = MockedEnv.CreateGuid(1),
                        Email = "philipe.souza@puchealth.com.br",
                        Name = "Philipe Souza",
                    },
                }, options => options.WithStrictOrdering()
            );
        }

        [Fact]
        public async Task PutClientNotFound()
        {
            // Arrange
            var data = new ClientPutView
            {
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza",
            };

            // Act
            var (response, _) = await PutResultAsString($"clients/{MockedEnv.CreateGuid(666)}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetClientListAfterPutNotFound()
        {
            // Arrange
            await PutClientNotFound();

            // Act
            var (response, actual) = await Get<List<ClientView>>("clients");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(IEnv.AdminClientView);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteNonExistent()
        {
            // Act
            var (response, _) = await Delete($"clients/{MockedEnv.CreateGuid(666)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteExistent()
        {
            // Arrange
            await PostClient();

            // Act
            var (response, _) = await Delete($"clients/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClientAfterDelete()
        {
            // Arrange
            await DeleteExistent();

            // Act
            var (response, _) = await GetResultAsString($"clients/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetClientListAfterDelete()
        {
            // Arrange
            await DeleteExistent();

            // Act
            var (response, actual) = await Get<List<ClientView>>("clients");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(IEnv.AdminClientView);
        }

        #endregion
    }
}