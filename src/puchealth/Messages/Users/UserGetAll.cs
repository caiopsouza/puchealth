using System.Collections.Generic;
using MediatR;
using puchealth.Views.Users;

namespace puchealth.Messages.Users
{
    public class UserGetAll : IRequest<IEnumerable<UserView>>
    {
    }
}