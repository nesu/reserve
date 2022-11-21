using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API.Requests.Account
{
    public static class Register
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<Unit>
        {
            public string Email { get; set; }
            
            public string Password { get; set; }
            
            public string PasswordConfirmation { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Email)
                    .NotEmpty().WithMessage("Please specify your email address.")
                    .EmailAddress().WithMessage("Incorrect email address format.");

                RuleFor(it => it.Password)
                    .NotEmpty().WithMessage("Password field is required.")
                    .Equal(it => it.PasswordConfirmation).WithMessage("Passwords must match.");

                RuleFor(it => it.Email)
                    .Must(vh.IsAccountUnique)
                    .WithMessage("Account with this email address already exists.");
            }
        }
        
        [UsedImplicitly]
        public class RequestHandler : IRequestHandler<RequestEnvelope, Unit>
        {
            private readonly DatabaseContext _context;

            public RequestHandler(DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<Unit> Handle(RequestEnvelope request_envelope, CancellationToken ct)
            {
                var account = new Data.Models.Account
                {
                    Email = request_envelope.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request_envelope.Password)
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync(ct);
                
                return Unit.Value;
            }
        }
    }
}