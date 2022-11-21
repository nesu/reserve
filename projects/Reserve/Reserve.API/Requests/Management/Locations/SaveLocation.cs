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

namespace Reserve.API.Requests.Management.Locations
{
    public static class SaveLocation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceLocationResource>
        {
            public long Id { get; set; }
            
            public string Label { get; set; }
            
            public string Address { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .NotEmpty().WithMessage("Bad request")
                    .Must(vh.ServiceLocationExists).WithMessage("Could not find specified service location");

                RuleFor(it => it.Label)
                    .NotEmpty().WithMessage("Location name cannot be empty");
                
                RuleFor(it => it.Address)
                    .NotEmpty().WithMessage("Location address cannot be empty");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceLocationResource>
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
            
            public async Task<ServiceLocationResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var location = await _context.ServiceLocations
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                location.Label = request.Label;
                location.Address = request.Address;

                _context.ServiceLocations.Attach(location);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<ServiceLocationResource>(location);
            }
        }
    }
}