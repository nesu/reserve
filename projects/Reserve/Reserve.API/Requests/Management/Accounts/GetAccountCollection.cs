using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Abstractions;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Management.Accounts
{
    public static class GetAccountCollection
    {
        [PublicAPI]
        public class RequestEnvelope : PageRequest<AccountResource[]>
        {
            public RoleType? Role { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, AccountResource[]>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<AccountResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var accounts = _context.Accounts
                    .AsQueryable();

                if (request.Role.HasValue)
                {
                    accounts = accounts
                        .Where(it => it.Role == request.Role.Value);
                }

                // If page size is not zero - return finite amount of rows.
                if (request.PageSize > 0)
                {
                    accounts = accounts
                        .Skip((request.PageIndex - 1) * request.PageSize)
                        .Take(request.PageSize);
                }

                return await accounts
                    .ProjectTo<AccountResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);
            }
        }
    }
}