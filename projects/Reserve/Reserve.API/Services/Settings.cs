using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Services
{
    public class Settings
    {
        private readonly DatabaseContext _context;

        public Settings(DatabaseContext context)
        {
            _context = context;
        }

        public bool IsWeekendWorkday(long employee_id)
        {
            var setting = _context.EmployeeSettings
                .FirstOrDefault(it => it.EmployeeId == employee_id && it.Type == EmployeeSettingType.IncludeWeekend);

            if (setting == null)
                return false;

            return Convert.ToBoolean(setting.Value);
        }
        
        public TimeSpan GetEmployeeWorkdayStart(long employee_id)
        {
            var start = _context.EmployeeSettings
                .FirstOrDefault(it => it.EmployeeId == employee_id && it.Type == EmployeeSettingType.WorkdayStart);
            
            if (start == null)
                return GetGlobalWorkdayStart();

            return Convert.ToDateTime(start.Value).TimeOfDay;
        }
        
        public TimeSpan GetEmployeeWorkdayEnd(long employee_id)
        {
            var end = _context.EmployeeSettings
                .FirstOrDefault(it => it.EmployeeId == employee_id && it.Type == EmployeeSettingType.WorkdayEnd);
            
            if (end == null)
                return GetGlobalWorkdayEnd();

            return Convert.ToDateTime(end.Value).TimeOfDay;
        }

        public TimeSpan GetGlobalWorkdayStart()
        {
            var start = _context.GlobalSettings
                .FirstOrDefault(it => it.Type == SettingType.WorkdayStart);
            
            if (start == null)
                return TimeSpan.FromHours(8);

            return Convert.ToDateTime(start.Value).TimeOfDay;
        }
        
        public TimeSpan GetGlobalWorkdayEnd()
        {
            var end = _context.GlobalSettings
                .FirstOrDefault(it => it.Type == SettingType.WorkdayEnd);
            
            if (end == null)
                return TimeSpan.FromHours(18);

            return Convert.ToDateTime(end.Value).TimeOfDay;
        }
    }
}