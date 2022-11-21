using System;
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
    public static class DeleteLocation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<Unit>
        {
            public long Id { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .NotEmpty().WithMessage("Bad request")
                    .Must(vh.ServiceLocationExists).WithMessage("Could not find specified service location");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, Unit>
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
            
            public async Task<Unit> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var location = await _context.ServiceLocations
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                var assigned = await _context.ServiceAssignees
                    .AnyAsync(it => it.LocationId == location.Id, ct);

                if (assigned)
                    throw new ValidationException("Location is currently being used by assignee");;
                
                _context.ServiceLocations.Remove(location);
                await _context.SaveChangesAsync(ct);

                return Unit.Value;
            }
        }
    }
}