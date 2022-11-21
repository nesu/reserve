using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Models;

namespace Reserve.API.Requests.Management.Categories
{
    public static class CreateCategory
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceCategoryResource>
        {
            public string Label { get; set; }
            
            public long? ParentCategoryId { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Label)
                    .NotEmpty().WithMessage("Category name cannot be empty");
                
                RuleFor(it => it.Label)
                    .MinimumLength(3).WithMessage("Category name must consist of at least 3 symbols");
                
                When(it => it.ParentCategoryId != null, () =>
                {
                    RuleFor(it => it.ParentCategoryId)
                        .Must(vh.ParentCategoryExists)
                        .WithMessage("Invalid parent category");
                });
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
                var category = new ServiceCategory
                {
                    Label = request.Label,
                    ParentCategoryId = request.ParentCategoryId
                };

                await _context.ServiceCategories.AddAsync(category, ct);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<ServiceCategoryResource>(category);
            }
        }
    }
}