using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Handlers;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Models;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Common
{
    public static class GetReservation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ReservationResource>
        {
            public long Id { get; set; }
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, ReservationResource>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(
                DatabaseContext context,
                IMapper mapper,
                IHttpContextAccessor accessor) : base(accessor)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public override async Task<ReservationResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var reservations = _context.Reservations
                    .Include(it => it.Services)
                    .ThenInclude(it => it.Service)
                    .Include(it => it.Assignee)
                    .Include(it => it.Account)
                    .Include(it => it.Event)
                    .AsQueryable();

                Reservation reservation;
                
                if (Role == RoleType.Client)
                {
                    reservation = await reservations
                        .FirstOrDefaultAsync(it => 
                            it.Id == request.Id &&
                            it.Account.Email == SessionIdentifier, ct
                        );
                }
                else
                {
                    reservation = await reservations
                        .FirstOrDefaultAsync(it => it.Id == request.Id, ct);   
                }

                if (reservation == null)
                    throw new ArgumentNullException(nameof(reservation));
                
                return _mapper
                    .Map<ReservationResource>(reservation);
            }
        }
    }
}