using System;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table("reservation_events")]
    public class ReservationEvent : BaseEntity
    {
        public long? AssigneeId { get; set; }
        
        public DateTime StartAt { get; set; }
        
        public DateTime EndAt { get; set; }
        
        public Account Assignee { get; set; }
    }
}