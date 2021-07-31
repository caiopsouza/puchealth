using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using puchealth.Models;
using puchealth.Persistence;
using puchealth.Services;

namespace puchealth.Controllers
{
    [AutoMap(typeof(Product))]
    public class BookmarkView
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = null!;

        public Uri Image { get; init; } = null!;

        public decimal Price { get; init; }

        public double? ReviewScore { get; init; } = null!;
    }

    [Route("v1/[controller]")]
    public class BookmarksController : Controller
    {
        private readonly Context _context;

        private readonly IMapper _mapper;

        public BookmarksController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private Guid GetUserLoggedIn() =>
            new(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("")]
        public IAsyncEnumerable<BookmarkView> GetAll() =>
            (
                from bookmark in _context.Bookmark
                where bookmark.ClientId == GetUserLoggedIn()
                let product = bookmark.Product
                orderby product.Title, product.Id
                select product
            )
            .ProjectTo<BookmarkView>(_mapper.ConfigurationProvider)
            .AsAsyncEnumerable();

        [HttpPost]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("{productId:guid}")]
        [ProducesResponseType(typeof(BookmarkView), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post(Guid productId)
        {
            var bookmark = new Bookmark
            {
                ClientId = GetUserLoggedIn(),
                ProductId = productId,
                CreatedAt = DateTime.Now,
            };

            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();

            var res = _context.Product
                .ProjectTo<BookmarkView>(_mapper.ConfigurationProvider)
                .First(p => p.Id == productId);

            return Created($"v1/bookmarks/{productId}", res);
        }

        [HttpDelete]
        [Authorize(Roles = IEnv.RoleAny)]
        [Route("{productId:guid}")]
        [ProducesResponseType(typeof(BookmarkView), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid productId)
        {
            var bookmark = new Bookmark
            {
                ClientId = GetUserLoggedIn(),
                ProductId = productId,
            };

            _context.Bookmark.Remove(bookmark);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}