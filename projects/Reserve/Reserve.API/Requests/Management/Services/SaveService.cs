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
using Reserve.Data.Models;

namespace Reserve.API.Requests.Management.Services
{
    public static class SaveService
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceResource>
        {
            public long Id { get; set; }
            
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
                var service = await _context.Services
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                service.Label = request.Label;
                service.Description = request.Description;
                service.Duration = request.Duration;
                service.Price = request.Price;
                service.CategoryId = request.CategoryId;
    
                _context.Services.Attach(service);
                await _context.SaveChangesAsync(ct);

                return _mapper
                    .Map<ServiceResource>(service);
            }
        }
    }
}
