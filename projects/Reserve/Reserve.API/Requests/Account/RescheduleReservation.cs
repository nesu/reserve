using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Handlers;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Account
{
    public static class RescheduleReservation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ReservationResource>
        {
            public long ReservationId { get; set; }
            
            public DateTime Datetime { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            private readonly DatabaseContext _context;

            public RequestValidator(
                DatabaseContext context)
            {
                _context = context;

                RuleFor(it => it.ReservationId)
                    .Must(ReservationExists).WithMessage("Could not find specified reservation");
            }

            private bool ReservationExists(long reservation_id)
            {
                return _context
                    .Reservations.Any(it => it.Id == reservation_id);
            }
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, ReservationResource>
        {
            private readonly DatabaseContext _context;
            private readonly Availability _availability;
            private readonly IMapper _mapper;

            public Handler(
                IHttpContextAccessor accessor,
                DatabaseContext context,
                Availability availability,
                IMapper mapper) : base(accessor)
            {
                _context = context;
                _availability = availability;
                _mapper = mapper;
            }

            public override async Task<ReservationResource> Handle(RequestEnvelope request, CancellationToken ct)
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

                var timespan = await _context.ReservedServices
                    .Where(it => it.ReservationId == reservation.Id)
                    .Include(it => it.Service)
                    .SumAsync(it => it.Service.Duration, ct);
                
                if (timespan > 300 || timespan < 15)
                    throw new ValidationException("Invalid service duration");
                
                var start = request.Datetime;
                var end = request.Datetime.AddMinutes(timespan);
                    
                if (!_availability.IsReservable(start, timespan, reservation.AssigneeId))
                    throw new ValidationException("Selected date and time is unavailable");

                var revent = reservation.Event;

                revent.StartAt = start;
                revent.EndAt = end;

                _context.ReservationEvents.Attach(revent);
                await _context.SaveChangesAsync(ct);

                return _mapper.Map<ReservationResource>(reservation);
            }
        }
    }
}