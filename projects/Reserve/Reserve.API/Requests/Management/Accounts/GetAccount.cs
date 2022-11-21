using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public static class GetAccount
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<AccountResource>
        {
            public long Id { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .Must(vh.AccountExists)
                    .WithMessage("Account does not exist.");
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

                return _mapper
                    .Map<AccountResource>(account);
            }
        }
    }
}