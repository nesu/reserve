using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Layout
{
    public static class GetPageByIdentifier
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<PageResource>
        {
            public string Identifier { get; set; }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, PageResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PageResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var page = await _context.Pages
                    .ProjectTo<PageResource>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(it => it.Identifier == request.Identifier, ct);
                
                if (page == null)
                    throw new ArgumentNullException(nameof(page));

                return page;
            }
        }
    }
}