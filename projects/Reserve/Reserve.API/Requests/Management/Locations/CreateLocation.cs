using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Models;

namespace Reserve.API.Requests.Management.Locations
{
    public static class CreateLocation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceLocationResource>
        {
            public string Label { get; set; }
            
            public string Address { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator()
            {
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
                var location = new ServiceLocation
                {
                    Label = request.Label,
                    Address = request.Address
                };

                await _context.ServiceLocations.AddAsync(location, ct);
                await _context.SaveChangesAsync(ct);

                return _mapper
                    .Map<ServiceLocationResource>(location);
            }
        }
    }
}