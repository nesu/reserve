using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Services
{
    public static class GetServiceCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceResource[]> { }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceResource[]>
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
            
            public async Task<ServiceResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var services = await _context.Services
                    .Include(it => it.Assignees)
                    .ThenInclude(it => it.Location)
                    .Include(it => it.Category)
                    .ProjectTo<ServiceResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);

                return services;
            }
        }
    }
}