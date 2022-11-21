using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Services
{
    public static class DeleteService
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
                    .NotEmpty().WithMessage("Bad request");

                RuleFor(it => it.Id)
                    .Must(vh.ServiceExists).WithMessage("Service does not exist");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, Unit>
        {
            private readonly DatabaseContext _context;

            public Handler(
                DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<Unit> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var service = await _context.Services
                    .Include(it => it.Assignees)
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                if (service != null)
                {
                    if (service.Assignees.Count > 0)
                    {
                        _context.ServiceAssignees.RemoveRange(service.Assignees);
                        await _context.SaveChangesAsync(ct);
                    }
                    
                    _context.Services.Remove(service);
                    await _context.SaveChangesAsync(ct);
                }

                return Unit.Value;
            }
        }
    }
}