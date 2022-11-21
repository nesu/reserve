using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Models;

namespace Reserve.API.Requests.Management.Locations
{
    public static class GetLocationCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceLocationResource[]> { }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceLocationResource[]>
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
            
            public async Task<ServiceLocationResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var locations = await _context.ServiceLocations
                    .ProjectTo<ServiceLocationResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);

                return locations;
            }
        }
    }
}