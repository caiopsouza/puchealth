using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using puchealth;
using puchealth.Messages.Users;
using puchealth.Services;
using puchealth.Views.Users;
using Tests.Setup;
using Xunit;

namespace Tests
{
    public class UserTests : IntegrationTest
    {
        public UserTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public static TheoryData<UserPost, IEnumerable<IdentityError>> AccountInfoErrorData =>
            new()
            {
                {
                    new UserPost
                    {
                        Name = "Caio Souza",
                        Email = "admin@puchealth.com.br",
                        Password = "Passsssssw0rd!"
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "DuplicateUserName",
                            Description = "Username 'admin@puchealth.com.br' is already taken."
                        }
                    }
                },

                {
                    new UserPost
                    {
                        Name = "Caio Souza",
                        Email = "caio.souza.puchealth.com.br",
                        Password = "Passsssssw0rd!"
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "InvalidEmail", Description = "Email 'caio.souza.puchealth.com.br' is invalid."
                        }
                    }
                }
            };

        public static TheoryData<UserPost, IEnumerable<IdentityError>> PasswordErrorData =>
            new()
            {
                {
                    new UserPost
                    {
                        Name = "Caio Souza",
                        Email = "caio.souza@puchealth.com.br",
                        Password = "word"
                    },
                    new[]
                    {
                        new IdentityError
                        {
                            Code = "PasswordTooShort",
                            Description = "Passwords must be at least 8 characters."
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresNonAlphanumeric",
                            Description = "Passwords must have at least one non alphanumeric character."
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresDigit",
                            Description = "Passwords must have at least one digit ('0'-'9')."
                        },
                        new IdentityError
                        {
                            Code = "PasswordRequiresUpper",
                            Description = "Passwords must have at least one uppercase ('A'-'Z')."
                        }
                    }
                }
            };

        [Fact]
        public async Task GetUsers()
        {
            // Act
            var (res, actual) = await Get<List<UserView>>("users");

            // Assert
            res.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(
                new[] {IEnv.AdminUserView, IEnv.ProfissionalUserView, IEnv.SuperAdminUserView},
                config => config.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(AccountInfoErrorData))]
        [MemberData(nameof(PasswordErrorData))]
        public async Task PostUserError(UserPost data, IEnumerable<IdentityError> expected)
        {
            // Act
            var (response, actual) = await Post<List<IdentityError>>("users", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
        }

        #region Post

        [Fact]
        public async Task PostUser()
        {
            // Arrange
            var data = new UserPost
            {
                Email = "caio.souza@puchealth.com.br",
                Name = "Caio Souza",
                Password = "Passw0000000rd!"
            };

            // Act
            var (response, actual) = await Post<UserView>("users", data);

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new UserView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = data.Email,
                Name = data.Name
            });
        }

        [Fact]
        public async Task GetUserAfterPost()
        {
            // Arrange
            await PostUser();

            // Act
            var (response, actual) = await Get<UserView>($"users/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new UserView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = "caio.souza@puchealth.com.br",
                Name = "Caio Souza"
            });
        }

        [Fact]
        public async Task GetUserListAfterPost()
        {
            // Arrange
            await PostUser();

            // Act
            var (response, actual) = await Get<List<UserView>>("users");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new[]
                {
                    IEnv.AdminUserView,
                    new UserView
                    {
                        Id = MockedEnv.CreateGuid(1),
                        Email = "caio.souza@puchealth.com.br",
                        Name = "Caio Souza"
                    },
                    IEnv.ProfissionalUserView,
                    IEnv.SuperAdminUserView
                }, options => options.WithStrictOrdering()
            );
        }

        #endregion

        #region Put

        [Theory]
        [MemberData(nameof(AccountInfoErrorData))]
        public async Task PutUserError(UserPost data, IEnumerable<IdentityError> expected)
        {
            // Arrange
            await PostUser();

            // Act
            var (response, actual) = await Put<List<IdentityError>>($"users/{MockedEnv.CreateGuid(1)}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task PutUser()
        {
            // Arrange
            await PostUser();

            var data = new UserPut
            {
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza"
            };

            // Act
            var (response, actual) = await Put<UserView>($"users/{MockedEnv.CreateGuid(1)}", data);

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new UserView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = data.Email,
                Name = data.Name
            });
        }

        [Fact]
        public async Task GetUserAfterPut()
        {
            // Arrange
            await PutUser();

            // Act
            var (response, actual) = await Get<UserView>($"users/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new UserView
            {
                Id = MockedEnv.CreateGuid(1),
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza"
            });
        }

        [Fact]
        public async Task GetUserListAfterPut()
        {
            // Arrange
            await PutUser();

            // Act
            var (response, actual) = await Get<List<UserView>>("users");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(new[]
                {
                    IEnv.AdminUserView,
                    new UserView
                    {
                        Id = MockedEnv.CreateGuid(1),
                        Email = "philipe.souza@puchealth.com.br",
                        Name = "Philipe Souza"
                    },
                    IEnv.ProfissionalUserView,
                    IEnv.SuperAdminUserView
                }, options => options.WithStrictOrdering()
            );
        }

        [Fact]
        public async Task PutUserNotFound()
        {
            // Arrange
            var data = new UserPut
            {
                Email = "philipe.souza@puchealth.com.br",
                Name = "Philipe Souza"
            };

            // Act
            var (response, _) = await PutResultAsString($"users/{MockedEnv.CreateGuid(666)}", data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUserListAfterPutNotFound()
        {
            // Arrange
            await PutUserNotFound();

            // Act
            var (response, actual) = await Get<List<UserView>>("users");

            // Assert
            response.EnsureSuccessStatusCode();
            actual.Should().BeEquivalentTo(
                new[] {IEnv.AdminUserView, IEnv.ProfissionalUserView, IEnv.SuperAdminUserView},
                config => config.WithStrictOrdering());
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteNonExistent()
        {
            // Act
            var (response, _) = await Delete($"users/{MockedEnv.CreateGuid(666)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteExistent()
        {
            // Arrange
            await PostUser();

            // Act
            var (response, _) = await Delete($"users/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetUserAfterDelete()
        {
            // Arrange
            await DeleteExistent();

            // Act
            var (response, _) = await GetResultAsString($"users/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUserListAfterDelete()
        {
            // Arrange
            await DeleteExistent();

            // Act
            var (response, actual) = await Get<List<UserView>>("users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actual.Should().BeEquivalentTo(
                new[] {IEnv.AdminUserView, IEnv.ProfissionalUserView, IEnv.SuperAdminUserView},
                config => config.WithStrictOrdering());
        }

        #endregion
    }
}