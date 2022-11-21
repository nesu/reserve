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
    public static class SaveCategory
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceCategoryResource>
        {
            public long Id { get; set; }
            
            public string Label { get; set; }
            
            public long? ParentCategoryId { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .Must(vh.CategoryExists)
                    .WithMessage("Invalid category");
                
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
                var category = await _context.ServiceCategories
                    .Include(it => it.ChildCategories)
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                category.Label = request.Label;
                category.ParentCategoryId = request.ParentCategoryId;

                _context.ServiceCategories.Attach(category);
                await _context.SaveChangesAsync(ct);

                return _mapper
                    .Map<ServiceCategoryResource>(category);
            }
        }
    }
}