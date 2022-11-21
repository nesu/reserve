using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management
{
    public static class GetCalendarEventCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<List<ResponseEnvelope>>
        {
            public long? EmployeeId { get; set; }
        }

        [PublicAPI]
        public class ResponseEnvelope
        {
            public string Name { get; set; }
            
            public string Start { get; set; }
            
            public string End { get; set; }
            
            public long EventId { get; set; }
            
            public long ReservationId { get; set; }
            
            public IEnumerable<ServiceResource> Services { get; set; }
            
            public AccountResource Account { get; set; }
            
            public AccountResource Employee { get; set; }
        }
        
        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, List<ResponseEnvelope>>
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
            
            public async Task<List<ResponseEnvelope>> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var reservations = _context.Reservations
                    .Include(it => it.Services)
                    .ThenInclude(it => it.Service)
                    .Include(it => it.Event)
                    .Include(it => it.Assignee)
                    .Include(it => it.Account)
                    .AsQueryable();

                if (request.EmployeeId.HasValue)
                {
                    reservations = reservations
                        .Where(it => it.AssigneeId == request.EmployeeId);
                }

                var resources = await reservations
                    .Select(it => _mapper.Map<ReservationResource>(it))
                    .ToListAsync(ct);
                
                var results = new List<ResponseEnvelope>();
                
                foreach (var resource in resources)
                {
                    var it = new ResponseEnvelope
                    {
                        Name = $"{resource.Account.GivenName} {resource.Account.FamilyName}",
                        Account = resource.Account,
                        Employee = resource.Assignee,
                        EventId = resource.EventId,
                        ReservationId = resource.Id,
                        Services = resource.Services.Select(rt => rt.Service),
                        Start = resource.Event.StartAt.ToString("yyyy-MM-dd HH:mm"),
                        End = resource.Event.EndAt.ToString("yyyy-MM-dd HH:mm")
                    };
                    
                    results.Add(it);
                }

                return results;
            }
        }
    }
}