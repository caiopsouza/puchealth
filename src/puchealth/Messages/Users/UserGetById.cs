using System;
using MediatR;
using puchealth.Views.Users;

namespace puchealth.Messages.Users
{
    public class UserGetById : IRequest<UserView?>
    {
        public UserGetById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}