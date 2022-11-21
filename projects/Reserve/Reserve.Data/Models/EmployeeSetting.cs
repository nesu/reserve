using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reserve.Data.Types;

namespace Reserve.Data.Models
{
    [Table("employee_settings")]
    public class EmployeeSetting : BaseEntity
    {
        public long EmployeeId { get; set; }
        
        public EmployeeSettingType Type { get; set; }
        
        [MaxLength(256)]
        public string Value { get; set; }

        public virtual Account Employee { get; set; }
    }
}