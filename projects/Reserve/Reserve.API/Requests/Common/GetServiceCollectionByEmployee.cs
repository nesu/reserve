using System;
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

namespace Reserve.API.Requests.Common
{
    public static class GetServiceCollectionByEmployee
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ServiceResource[]>
        {
            public long EmployeeId { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.EmployeeId)
                    .Must(vh.AccountExists).WithMessage("Specialist does not exist");
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ServiceResource[]>
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
            
            public async Task<ServiceResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var assignments = await _context.ServiceAssignees
                    .Include(it => it.Service)
                    .ThenInclude(it => it.Category)
                    .ThenInclude(it => it.ChildCategories)
                    .Where(it => it.AssigneeId == request.EmployeeId)
                    .Select(it => it.Service)
                    .ProjectTo<ServiceResource>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(ct);

                return assignments;
            }
        }
    }
}