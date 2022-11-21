using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Layout
{
    public static class GetPageCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<PageResource[]> { }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, PageResource[]>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PageResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                return await _context.Pages
                    .ProjectTo<PageResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);
            }
        }
    }
}