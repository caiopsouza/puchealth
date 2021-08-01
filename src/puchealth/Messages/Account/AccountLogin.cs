using MediatR;

namespace puchealth.Messages.Account
{
    public class AccountLogin : IRequest<string>
    {
        public string Email { get; init; } = default!;

        public string Password { get; init; } = default!;
    }
}