using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Management.Categories;
using Reserve.API.Services;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Accounts
{
    public static class DeleteAccount
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<Unit>
        {
            public long Id { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<GetCategoryById.RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .Must(vh.AccountExists)
                    .WithMessage("Invalid category");
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
                var category = await _context.ServiceCategories
                    .Include(it => it.ChildCategories)
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);

                if (category.ChildCategories.Count > 0) 
                {
                    _context.RemoveRange(category.ChildCategories);
                    await _context.SaveChangesAsync(ct);
                }

                _context.Remove(category);
                await _context.SaveChangesAsync(ct);
                
                return Unit.Value;
            }
        }
    }
}