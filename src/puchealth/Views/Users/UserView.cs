using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views.Users
{
    [AutoMap(typeof(User))]
    public class UserView
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;
    }
}