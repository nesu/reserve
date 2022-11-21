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

namespace Reserve.API.Requests.Management.Services
{
    public static class CreateService
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceResource>
        {
            public string Label { get; set; }
            
            public string Description { get; set; }
            
            public double Price { get; set; }
            
            public int Duration { get; set; }
            
            public long CategoryId { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Label)
                    .NotEmpty().WithMessage("Service name is required");

                RuleFor(it => it.Price)
                    .NotEmpty().WithMessage("Service price is not set");

                RuleFor(it => it.Duration)
                    .NotEmpty().WithMessage("Service duration is not set");

                RuleFor(it => it.CategoryId)
                    .NotEmpty().WithMessage("Service must have assigned category")
                    .Must(vh.CategoryExists).WithMessage("Provided category does not exist");
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceResource>
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
            
            public async Task<ServiceResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var service = new Service
                {
                    Label = request.Label,
                    Description = request.Description,
                    Duration = request.Duration,
                    Price = request.Price,
                    CategoryId = request.CategoryId
                };

                await _context.Services.AddAsync(service, ct);
                await _context.SaveChangesAsync(ct);

                return _mapper
                    .Map<ServiceResource>(service);
            }
        }
    }
}
