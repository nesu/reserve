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
using Reserve.Data.Models;

namespace Reserve.API.Requests.Management.Services
{
    public static class CreateServiceAssignment
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceAssigneeResource>
        {
            public long ServiceId { get; set; }
            
            public long AssigneeId { get; set; }
            
            public long LocationId { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            private readonly DatabaseContext _context;

            public RequestValidator(
                DatabaseContext context,
                ValidationHelpers vh)
            {
                _context = context;
                
                RuleFor(it => it.ServiceId)
                    .NotEmpty().WithMessage("Service ID was not provided")
                    .Must(vh.ServiceExists).WithMessage("Provided service does not exist");
                
                RuleFor(it => it.AssigneeId)
                    .NotEmpty().WithMessage("Assignee ID was not provided")
                    .Must(vh.AccountExists).WithMessage("Provided assignee does not exist");
                
                RuleFor(it => it.LocationId)
                    .NotEmpty().WithMessage("Location ID was not provided")
                    .Must(vh.ServiceLocationExists).WithMessage("Provided location does not exist");

                RuleFor(it => it)
                    .Must(IsUniqueAssignment).WithMessage("Cannot create duplicate assignment");
            }

            private bool IsUniqueAssignment(RequestEnvelope request)
            {
                var result = _context.ServiceAssignees
                    .Any(it => it.ServiceId == request.ServiceId
                               && it.AssigneeId == request.AssigneeId
                               && it.LocationId == request.LocationId);

                return !result;
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceAssigneeResource>
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
            
            public async Task<ServiceAssigneeResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var assignee = new ServiceAssignee
                {
                    ServiceId = request.ServiceId,
                    AssigneeId = request.AssigneeId,
                    LocationId = request.LocationId
                };

                await _context.ServiceAssignees.AddAsync(assignee, ct);
                await _context.SaveChangesAsync(ct);

                var result = await _context.ServiceAssignees
                    .Include(it => it.Assignee)
                    .Include(it => it.Location)
                    .Include(it => it.Service)
                    .FirstOrDefaultAsync(it => it.Id == assignee.Id, ct);

                return _mapper
                    .Map<ServiceAssigneeResource>(result);
            }
        }
    }
}