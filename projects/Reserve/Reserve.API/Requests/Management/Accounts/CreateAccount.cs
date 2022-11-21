using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Requests.Mailing;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Management.Accounts
{
    public static class CreateAccount
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<AccountResource>
        {
            public string Email { get; set; }

            public string GivenName { get; set; }
            
            public string FamilyName { get; set; }
            
            public string PhoneNumber { get; set; }
            
            public RoleType Role { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Email)
                    .NotEmpty().WithMessage("Please specify account email address.")
                    .EmailAddress().WithMessage("Incorrect email address format.");

                RuleFor(it => it.Email)
                    .Must(vh.IsAccountUnique)
                    .WithMessage("Account with this email address already exists.");
                
                RuleFor(it => it.Role)
                    .IsInEnum()
                    .WithMessage("Invalid specified account role.");
            }
        }
        
        [UsedImplicitly]
        public class RequestHandler : IRequestHandler<RequestEnvelope, AccountResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMediator _mediator;
            private readonly IMapper _mapper;

            public RequestHandler(
                DatabaseContext context,
                IMediator mediator,
                IMapper mapper)
            {
                _context = context;
                _mediator = mediator;
                _mapper = mapper;
            }
            
            public async Task<AccountResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var password = RandomString(16);
                var account = new Data.Models.Account
                {
                    Email = request.Email,
                    FamilyName = request.FamilyName,
                    GivenName = request.GivenName,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = request.Role
                };
                
                await _context.Accounts.AddAsync(account, ct);
                await _context.SaveChangesAsync(ct);
                
                var mailing = new SendAccountCreated.NotificationEnvelope
                {
                    AccountEmail = request.Email,
                    GeneratedPassword = password
                };

                await _mediator.Publish(mailing, ct);

                return _mapper.Map<AccountResource>(account);;
            }
            
            private string RandomString(int length)
            {
                var random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }
}