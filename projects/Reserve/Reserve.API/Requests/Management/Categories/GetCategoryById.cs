using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Categories
{
    public static class GetCategoryById
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceCategoryResource>
        {
            public long Id { get; set; }

            public RequestEnvelope(long id)
            {
                Id = id;
            }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .Must(vh.CategoryExists)
                    .WithMessage("Invalid category");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceCategoryResource>
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
            
            public async Task<ServiceCategoryResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var category = await _context.ServiceCategories
                    .Include(it => it.ChildCategories)
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                return _mapper.Map<ServiceCategoryResource>(category);
            }
        }
    }
}