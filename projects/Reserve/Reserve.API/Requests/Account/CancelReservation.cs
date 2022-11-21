using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Handlers;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Account
{
    public static class CancelReservation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<Unit>
        {
            public long ReservationId { get; set; }
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, Unit>
        {
            private readonly DatabaseContext _context;

            public Handler(
                IHttpContextAccessor accessor,
                DatabaseContext context) : base(accessor)
            {
                _context = context;
            }

            public override async Task<Unit> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Email == SessionIdentifier, ct);
                
                var reservation = await _context.Reservations
                    .Include(it => it.Services)
                    .Include(it => it.Event)
                    .Include(it => it.Assignee)
                    .FirstOrDefaultAsync(it => it.Id == request.ReservationId, ct);
                
                if (Role == RoleType.Client && reservation.AccountId != account.Id)
                    throw new ArgumentNullException(nameof(reservation));

                if (Role == RoleType.Employee && reservation.AssigneeId != account.Id)
                    throw new ArgumentNullException(nameof(reservation));

                if (reservation == null)
                    throw new ArgumentNullException(nameof(reservation));

                if (reservation.Event != null)
                {
                    _context.ReservationEvents.Remove(reservation.Event);
                    await _context.SaveChangesAsync(ct);
                }

                return Unit.Value;
            }
        }
    }
}