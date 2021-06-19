using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using puchealth;
using puchealth.Controllers;
using Tests.Setup;
using Xunit;

namespace Tests
{
    public class BookmarkTests : IntegrationTest
    {
        private readonly BookmarkView _prod2 = new()
        {
            Id = new Guid("00000000-0002-0000-0000-000000000000"),
            Title = "Película Protetora para Samsung Galaxy S6",
            Image = new Uri("http://challenge-api.luizalabs.com/images/de2911eb-ce5c-e783-1ca5-82d0ccd4e3d8.jpg"),
            Price = 39.9m,
            ReviewScore = null,
        };

        private readonly BookmarkView _prod4 = new()
        {
            Id = new Guid("00000000-0004-0000-0000-000000000000"),
            Title = "Colcha/Cobre-Leito Patchwork Casal Camesa Curação",
            Image = new Uri("http://challenge-api.luizalabs.com/images/b5e7410b-cd4f-bb9d-3c95-49010fbee801.jpg"),
            Price = 159.75m,
            ReviewScore = 1.0,
        };

        public BookmarkTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            // Testing users
            CreateAlice().Wait();
            CreateBob().Wait();
        }

        private async Task<IEnumerable<BookmarkView>> GetBookmarks()
        {
            var (res, bookmarks) = await Get<IEnumerable<BookmarkView>>("bookmarks");
            res.EnsureSuccessStatusCode();
            return bookmarks;
        }

        private async Task<IEnumerable<BookmarkView>> GetBookmarksAlice()
        {
            await LoginAsAlice();
            return await GetBookmarks();
        }

        private async Task<IEnumerable<BookmarkView>> GetBookmarksBob()
        {
            await LoginAsBob();
            return await GetBookmarks();
        }

        [Fact]
        private async Task EmptyAlice()
        {
            // Act
            var actual = await GetBookmarksAlice();

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        private async Task EmptyBob()
        {
            // Act
            var actual = await GetBookmarksBob();

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task BookmarkExisting()
        {
            // Arrange
            await LoginAsAlice();

            // Act
            var (res2, actual2) = await Post<BookmarkView>($"bookmarks/{_prod2.Id.ToString()}");
            var (res4, actual4) = await Post<BookmarkView>($"bookmarks/{_prod4.Id.ToString()}");

            // Assert
            res2.EnsureSuccessStatusCode();
            res4.EnsureSuccessStatusCode();

            actual2.Should().BeEquivalentTo(_prod2);
            actual4.Should().BeEquivalentTo(_prod4);

            (await GetBookmarksBob()).Should().BeEmpty();

            (await GetBookmarksAlice()).Should().BeEquivalentTo(
                new[] {_prod4, _prod2},
                options => options.WithStrictOrdering()
            );
        }

        [Fact]
        public async Task DeleteExisting()
        {
            // Arrange
            await BookmarkExisting();
            await LoginAsAlice();

            // Act
            var (res, _) = await Delete($"bookmarks/{_prod4.Id.ToString()}");

            // Assert
            res.EnsureSuccessStatusCode();
            (await GetBookmarksAlice()).Should().BeEquivalentTo(_prod2);
        }

        [Fact]
        public async Task DeleteClientWithBookmarks()
        {
            // Arrange
            await BookmarkExisting();
            await LoginAsAdmin();

            // Act
            var (response, _) = await Delete($"clients/{MockedEnv.CreateGuid(1)}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await new Func<Task>(LoginAsAlice) // Can't log anymore
                .Should()
                .ThrowAsync<HttpRequestException>();
        }
    }
}