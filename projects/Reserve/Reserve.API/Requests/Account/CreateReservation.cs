using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Requests.Handlers;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Models;

namespace Reserve.API.Requests.Account
{
    public static class CreateReservation
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<Unit>
        {
            public long EmployeeId { get; set; }
            
            public DateTime Datetime { get; set; }
            
            public long[] Services { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.EmployeeId)
                    .Must(vh.AccountExists);

                RuleFor(it => it.Services.Length)
                    .GreaterThan(0).WithMessage("No services selected");
            }
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, Unit>
        {
            private readonly DatabaseContext _context;
            private readonly Availability _availability;

            public Handler(
                IHttpContextAccessor accessor,
                DatabaseContext context,
                Availability availability) : base(accessor)
            {
                _context = context;
                _availability = availability;
            }
            
            public override async Task<Unit> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var timespan = await _context.Services
                    .Where(it => request.Services.Contains(it.Id))
                    .SumAsync(it => it.Duration, ct);
                
                if (timespan > 300 || timespan < 15)
                    throw new ValidationException("Invalid service duration");

                var start = request.Datetime;
                var end = request.Datetime.AddMinutes(timespan);

                if (!_availability.IsReservable(start, timespan, request.EmployeeId))
                    throw new ValidationException("Selected date and time is unavailable");

                var revent = new ReservationEvent
                {
                    StartAt = start,
                    EndAt = end,
                    AssigneeId = request.EmployeeId
                };

                await _context.ReservationEvents.AddAsync(revent, ct);
                await _context.SaveChangesAsync(ct);

                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Email == SessionIdentifier, ct);
                
                if (account == null)
                    throw new ArgumentNullException(nameof(account));

                var reservation = new Reservation
                {
                    AccountId = account.Id,
                    AssigneeId = request.EmployeeId,
                    EventId = revent.Id
                };

                await _context.Reservations.AddAsync(reservation, ct);
                await _context.SaveChangesAsync(ct);

                foreach (var service in request.Services)
                {
                    var rser = new ReservedService
                    {
                        ReservationId = reservation.Id,
                        ServiceId = service
                    };
                    
                    await _context.ReservedServices.AddAsync(rser, ct);
                    await _context.SaveChangesAsync(ct);
                }
                
                return Unit.Value;
            }
        }
    }
}