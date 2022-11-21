using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Handlers;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Account
{
    public static class ChangePassword
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<EmptyResource>
        {
            public string CurrentPassword { get; set; }
            
            public string NewPassword { get; set; }
            
            public string ConfirmedNewPassword { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator()
            {
                // Validation messages are displayed in front-end. If someone attempts to bypass
                // front-end validation, do not bother informing them of request fault reason.
                RuleFor(it => it.CurrentPassword)
                    .NotEmpty();

                RuleFor(it => it.NewPassword)
                    .NotEmpty();

                RuleFor(it => it.ConfirmedNewPassword)
                    .NotEmpty()
                    .Equal(it => it.NewPassword);
            }
        }
        
        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, EmptyResource>
        {
            private readonly DatabaseContext _context;

            public Handler(
                DatabaseContext context,
                IHttpContextAccessor accessor) : base(accessor)
            {
                _context = context;
            }

            public override async Task<EmptyResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Email == SessionIdentifier, ct);
                
                if (account == null)
                    throw new ArgumentNullException(nameof(account));
                
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.PasswordHash))
                    throw new UnauthorizedAccessException();

                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                _context.Accounts.Attach(account);
                await _context.SaveChangesAsync(ct);
                
                return EmptyResource.Default;
            }
        }
    }
}