using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Account;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Layout
{
    public static class DeletePage
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
                var page = await _context.Pages
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);
                
                if (page == null)
                    throw new ArgumentNullException(nameof(page));

                _context.Pages.Remove(page);
                await _context.SaveChangesAsync(ct);
                
                return EmptyResource.Default;
            }
        }
    }
}