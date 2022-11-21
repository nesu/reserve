using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Management.Accounts
{
    public static class SaveAccount
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<AccountResource>
        {
            public long Id { get; set; }
            
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
                RuleFor(it => it.Id)
                    .Must(vh.AccountExists)
                    .WithMessage("Account does not exist.");
                
                RuleFor(it => it.Email)
                    .NotEmpty().WithMessage("Please specify your email address.")
                    .EmailAddress().WithMessage("Incorrect email address format.");

                RuleFor(it => it.Role)
                    .IsInEnum()
                    .WithMessage("Invalid specified account role.");
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, AccountResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(
                DatabaseContext context,
                IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<AccountResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                if (account.Email != request.Email)
                {
                    var exists = await _context.Accounts
                        .AnyAsync(it => it.Email == request.Email, ct);
                    
                    if (exists)
                        throw new ValidationException("Account with this email address already exists");
                }
                
                
                account.GivenName = request.GivenName;
                account.FamilyName = request.FamilyName;
                account.PhoneNumber = request.PhoneNumber;
                account.Role = request.Role;

                _context.Accounts.Attach(account);
                await _context.SaveChangesAsync(ct);

                return _mapper
                    .Map<AccountResource>(account);
            }
        }
    }
}