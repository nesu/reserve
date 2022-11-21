using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reserve.API.Configurations;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Management.Layout
{
    public static class EditNavigatorElement
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<EmptyResource>
        {
            public long Id { get; set; }
            
            public int Order { get; set; }
        
            public string Title { get; set; }
        
            public string MdiIcon { get; set; }
        
            public string RedirectTo { get; set; }
        
            public bool MobileView { get; set; }
        
            public bool GuestOnly { get; set; }
        
            public bool AuthenticatedOnly { get; set; }
            
            public Dictionary<string, object> Properties { get; set; }
        
            public RoleType[] PermittedRoles { get; set; }
        }
        
        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator()
            {
                RuleFor(it => it.Id)
                    .NotNull();
            
                RuleFor(it => it.Order)
                    .NotNull();

                RuleFor(it => it.Title)
                    .NotEmpty();

                RuleFor(it => it.MdiIcon)
                    .NotEmpty();

                RuleFor(it => it.RedirectTo)
                    .NotEmpty();
            }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, EmptyResource>
        {
            private readonly DatabaseContext _context;

            public Handler(DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<EmptyResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var element = await _context.VisualElements
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);
                
                if (element == null)
                    throw new ArgumentNullException(nameof(element));

                element.Properties =
                    JsonConvert.SerializeObject(request, SerializerConfigurations.DefaultSerializerSettings);

                _context.VisualElements.Update(element);
                await _context.SaveChangesAsync(ct);
                
                return EmptyResource.Default;
            }
        }
    }
}