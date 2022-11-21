using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table("service_assignees")]
    public class ServiceAssignee : BaseEntity,  ITimestamped
    {
        public long ServiceId { get; set; }
        
        public long AssigneeId { get; set; }
        
        public long LocationId { get; set; }
        
        public Service Service { get; set; }
        
        public Account Assignee { get; set; }
        
        public ServiceLocation Location { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}