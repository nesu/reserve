using System.Collections.Generic;
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

namespace Reserve.API.Requests.Account
{
    public static class GetReservationCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<IEnumerable<ReservationResource>> { }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, IEnumerable<ReservationResource>>
        {
            private readonly DatabaseContext _context;
            private readonly IMapper _mapper;

            public Handler(
                IHttpContextAccessor accessor,
                DatabaseContext context,
                IMapper mapper) : base(accessor)
            {
                _context = context;
                _mapper = mapper;
            }

            public override async Task<IEnumerable<ReservationResource>> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .Include(it => it.Reservations).ThenInclude(it => it.Services)
                    .ThenInclude(it => it.Service)
                    .Include(it => it.Reservations).ThenInclude(it => it.Assignee)
                    .Include(it => it.Reservations).ThenInclude(it => it.Event)
                    .FirstOrDefaultAsync(it => it.Email == SessionIdentifier, ct);

                var reservations = account.Reservations
                    .Select(it => _mapper.Map<ReservationResource>(it));

                return reservations;
            }
        }
    }
}