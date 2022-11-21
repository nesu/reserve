using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    public static class SavePersonalInformation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<AccountResource>
        {
            public string GivenName { get; set; }
            
            public string Email { get; set; }
            
            public string FamilyName { get; set; }
            
            public string PhoneNumber { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator()
            {
                // Validation messages are displayed in front-end. If someone attempts to bypass
                // front-end validation, do not bother informing them of request fault reason.
                RuleFor(it => it.GivenName)
                    .NotEmpty();

                RuleFor(it => it.FamilyName)
                    .NotEmpty();

                RuleFor(it => it.Email)
                    .EmailAddress();

                RuleFor(it => it.PhoneNumber)
                    .NotEmpty();
            }
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, AccountResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(
                DatabaseContext context, 
                IMapper mapper,
                IHttpContextAccessor accessor) : base(accessor)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public override async Task<AccountResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Email == SessionIdentifier, ct);
                
                if (account == null)
                    throw new ArgumentNullException(nameof(account));

                account.Email = request.Email;
                account.GivenName = request.GivenName;
                account.FamilyName = request.FamilyName;
                account.PhoneNumber = request.PhoneNumber;

                _context.Accounts.Attach(account);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<AccountResource>(account);
            }
        }
    }
}