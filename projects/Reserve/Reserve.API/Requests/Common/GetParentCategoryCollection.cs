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

namespace Reserve.API.Requests.Common
{
    public static class GetParentCategoryCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceCategoryResource[]> { }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceCategoryResource[]>
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
            
            public async Task<ServiceCategoryResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var categories = await _context.ServiceCategories
                    .Include(it => it.ChildCategories)
                    .Where(it => it.ParentCategoryId == null)
                    .ProjectTo<ServiceCategoryResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);

                return categories;
            }
        }
    }
}