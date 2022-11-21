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

namespace Reserve.API.Requests.Management.Layout
{
    public static class CreatePage
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<PageResource>
        {
            public string Title { get; set; }
            
            public string Identifier { get; set; }
            
            public string Contents { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Title)
                    .NotEmpty();
                
                RuleFor(it => it.Identifier)
                    .NotEmpty().WithMessage("New page must have identifier.")
                    .Must(vh.IsPageUnique).WithMessage("Page with this identifier already exists");
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, PageResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(DatabaseContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PageResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var page = new Page
                {
                    Title = request.Title,
                    Identifier = request.Identifier,
                    Contents = request.Contents
                };

                _context.Pages.Add(page);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<PageResource>(page);
            }
        }
    }
}