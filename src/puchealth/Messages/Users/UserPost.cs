using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneOf;
using puchealth.Models;
using puchealth.Views.Users;

namespace puchealth.Messages.Users
{
    [AutoMap(typeof(User), ReverseMap = true)]
    public class UserPost : IRequest<OneOf<UserView, IEnumerable<IdentityError>>>
    {
        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}