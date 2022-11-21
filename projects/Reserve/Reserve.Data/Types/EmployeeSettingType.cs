using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace Reserve.Data.Types
{
    public enum EmployeeSettingType
    {
        None = 0,
        WorkdayStart = 1,
        WorkdayEnd = 2,
        IncludeWeekend = 3
    }
}