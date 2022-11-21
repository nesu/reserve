using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Resources;
using Reserve.API.Services;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Common
{
    public static class GetAvailableTimespanCollection
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ResponseEnvelope>
        {
            public DateTime Date { get; set; }
            
            public int Duration { get; set; }
            
            public long EmployeeId { get; set; }
        }

        [PublicAPI]
        public class ResponseEnvelope
        {
            public List<string> Disabled { get; set; }
            
            public Dictionary<string, List<string>> Data { get; set; }
            
            [CanBeNull] 
            public string Nearest { get; set; }
        }

        [UsedImplicitly]
        public class RequestValidator : AbstractValidator<RequestEnvelope>
        {
            public RequestValidator(ValidationHelpers vh)
            {
                RuleFor(it => it.Duration)
                    .GreaterThanOrEqualTo(15).WithMessage("Minimum required reservation duration is 15 minutes")
                    .LessThanOrEqualTo(300).WithMessage("Maximum allowed reservation duration is 300 minutes");

                RuleFor(it => (it.Date - DateTime.Now).TotalDays)
                    .LessThan(90).WithMessage("Cannot lookup more than three months");
                
                RuleFor(it => it.EmployeeId)
                    .NotEmpty().WithMessage("Specialist must be provided")
                    .Must(vh.AccountExists).WithMessage("Provided specialist does not exist");
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ResponseEnvelope>
        {
            private readonly DatabaseContext _context;
            private readonly Availability _availability;

            public Handler(
                DatabaseContext context,
                Availability availability)
            {
                _context = context;
                _availability = availability;
            }
            
            public async Task<ResponseEnvelope> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var employee = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Id == request.EmployeeId, ct);
                
                if (employee.Role != RoleType.Employee)
                    throw new ArgumentNullException(nameof(employee));

                var year = request.Date.Year;
                var month = request.Date.Month;
                
                var start_of_month = new DateTime(year, month, 1);
                var end_of_month = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                ResponseEnvelope result = new ResponseEnvelope
                {
                    Nearest = null,
                    Data = new Dictionary<string, List<string>>(),
                    Disabled = new List<string>()
                };

                var period_array = GetPeriods(start_of_month, end_of_month);
                
                foreach (var period in period_array)
                {
                    var key = period.StartAt.ToString("yyyy-MM-dd");
                    var internal_periods =
                        _availability.CreatePeriods(period.StartAt, request.Duration, request.EmployeeId);

                    if (internal_periods.Count > 0)
                    {
                        result.Data[key] = new List<string>();
                        foreach (var internal_period in internal_periods)
                        {
                            result.Data[key].Add(internal_period.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        result.Nearest ??= key;
                    }
                    else
                    {
                        result.Disabled.Add(key);
                    }
                }

                return result;
            }
            
            private IEnumerable<PeriodResource> GetPeriods(DateTime start, DateTime end)
            {
                DateTime cursor = start;
            
                while (cursor < end)
                {
                    yield return new PeriodResource
                    {
                        StartAt = cursor, 
                        EndAt = cursor.AddDays(1)
                    };

                    cursor = cursor.AddDays(1);
                }
            }
        }
    }
}