using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Services
{
    public static class GetServiceAssignmentCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceAssigneeResource[]>
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
                    .NotEmpty().WithMessage("Service ID was not provided")
                    .Must(vh.ServiceExists).WithMessage("Provided service does not exist");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceAssigneeResource[]>
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
            
            public async Task<ServiceAssigneeResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var assignments = await _context.ServiceAssignees
                    .Where(it => it.ServiceId == request.Id)
                    .ProjectTo<ServiceAssigneeResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);

                return assignments;
            }
        }
    }
}