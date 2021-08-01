using System;
using AutoMapper;
using MediatR;
using puchealth.Models;

namespace puchealth.Messages.Users
{
    [AutoMap(typeof(User), ReverseMap = true)]
    public class UserDelete : IRequest<bool>
    {
        public UserDelete(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}