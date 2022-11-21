using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Reserve.API.Extensions;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Services
{
    public class Availability
    {
        private readonly DatabaseContext _context;
        private readonly Settings _settings;

        public Availability(
            DatabaseContext context,
            Settings settings)
        {
            _context = context;
            _settings = settings;
        }

        public List<DateTime> CreatePeriods(DateTime date, int duration, long employee_id)
        {
            var result = new List<DateTime>();
            var current = DateTime.Now;
            
            if (date < current || date.Date == current.Date || !IsWorkday(date, employee_id))
                return result;

            var workday_start_timespan = _settings.GetEmployeeWorkdayStart(employee_id);
            var workday_end_timespan = _settings.GetEmployeeWorkdayEnd(employee_id);

            var workday_start = date.SetTime(workday_start_timespan);
            var workday_end = date.SetTime(workday_end_timespan);
            
            // Check if there are events that intersects with whole working hours.
            var intersections = _context.ReservationEvents
                .Count(it => it.StartAt <= workday_start && it.EndAt >= workday_end);

            if (intersections > 0)
                return result;

            var periods = GetPeriods(workday_start, workday_end);
            foreach (var period in periods)
            {
                var event_start = period.StartAt;
                var event_end = period.StartAt.AddMinutes(duration);

                if (!(event_start >= workday_start && event_start <= workday_end) &&
                    !(event_end >= workday_start && event_end <= workday_end))
                    continue;

                if (IsAvailable(event_start, event_end, employee_id))
                    result.Add(event_start);
            }

            return result;
        }

        public bool IsReservable(DateTime start, int duration, long employee_id)
        {
            var current = DateTime.Now;
            if (start < current || start.Date == current.Date)
                return false;
            
            var end = start.AddMinutes(duration);

            var workday_start_timespan = _settings.GetEmployeeWorkdayStart(employee_id);
            var workday_end_timespan = _settings.GetEmployeeWorkdayEnd(employee_id);
            
            var workday_start = start.SetTime(workday_start_timespan);
            var workday_end = start.SetTime(workday_end_timespan);

            if (!(start >= workday_start && start <= workday_end) &&
                !(end >= workday_start && end <= workday_end))
                return false;

            return IsAvailable(start, end, employee_id);
        }

        private bool IsAvailable(DateTime start, DateTime end, long employee_id)
        {
            var intersections = _context.ReservationEvents
                .Count(it => (it.StartAt <= start && it.EndAt > start || it.StartAt > start && it.StartAt < end) &&
                             it.AssigneeId == employee_id);

            return intersections < 1;
        }

        private bool IsWorkday(DateTime date, long employee_id)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return _settings.IsWeekendWorkday(employee_id);
            
            return true;
        }
        
        private IEnumerable<PeriodResource> GetPeriods(DateTime start, DateTime end)
        {
            DateTime cursor = start;
            
            while (cursor < end)
            {
                yield return new PeriodResource
                {
                    StartAt = cursor, 
                    EndAt = cursor.AddMinutes(15)
                };

                cursor = cursor.AddMinutes(15);
            }
        }
    }
}