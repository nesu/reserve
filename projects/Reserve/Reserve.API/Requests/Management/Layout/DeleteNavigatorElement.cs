using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Layout
{
    public static class DeleteNavigatorElement
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<EmptyResource>
        {
            public long Id { get; set; }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, EmptyResource>
        {
            private readonly DatabaseContext _context;

            public Handler(DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<EmptyResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var element = await _context.VisualElements
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);
                
                if (element == null)
                    throw new ArgumentNullException(nameof(element));

                _context.VisualElements.Remove(element);
                await _context.SaveChangesAsync(ct);
                
                return EmptyResource.Default;;
            }
        }
    }
}