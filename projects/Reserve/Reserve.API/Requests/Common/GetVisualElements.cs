using System.Collections.Generic;
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
using Reserve.Data.Models;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Common
{
    public static class GetVisualElements
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<List<VisualElementResource>> { }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, List<VisualElementResource>>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext context, IMapper mapper) {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<VisualElementResource>> Handle(RequestEnvelope request, CancellationToken ct)
            {
                return await _context
                    .VisualElements
                    .Select(it => _mapper.Map<VisualElementResource>(it))
                    .ToListAsync(ct);
            }
        }
    }
}