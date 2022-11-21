using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Common
{
    public static class GetCompactEmployeeCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<CompactEmployeeResource[]>
        {
            public long LocationId { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, CompactEmployeeResource[]>
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
            
            public async Task<CompactEmployeeResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var employees = await _context.ServiceAssignees
                    .Where(it => it.LocationId == request.LocationId)
                    .Include(it => it.Assignee)
                    .Select(it => it.Assignee)
                    .ProjectTo<CompactEmployeeResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);
                
                return employees;
            }
        }
    }
}