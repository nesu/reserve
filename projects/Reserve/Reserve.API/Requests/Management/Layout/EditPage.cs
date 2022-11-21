using System;
using System.Data;
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

namespace Reserve.API.Requests.Management.Layout
{
    public static class EditPage
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<PageResource>
        {
            public long Id { get; set; }
            
            public string Identifier { get; set; }
            
            public string Title { get; set; }
            
            public string Contents { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Id)
                    .NotEmpty();

                RuleFor(it => it.Identifier)
                    .NotEmpty().WithMessage("New page must have identifier.");
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, PageResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;
            private readonly ValidationHelpers _vh;

            public Handler(DatabaseContext context, IMapper mapper, ValidationHelpers vh)
            {
                _context = context;
                _mapper = mapper;
                _vh = vh;
            }
            
            public async Task<PageResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var page = await _context.Pages
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);
                
                if (page == null)
                    throw new ArgumentNullException(nameof(page));

                if (page.Identifier != request.Identifier)
                {
                    if (!_vh.IsPageUnique(request.Identifier))
                        throw new DuplicateNameException(nameof(request.Identifier));

                    page.Identifier = request.Identifier;
                }

                page.Title = request.Title;
                page.Contents = request.Contents;

                _context.Pages.Attach(page);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<PageResource>(page);
            }
        }
    }
}