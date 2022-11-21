using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace Reserve.Data.Models
{
    [Table("reserved_services")]
    public class ReservedService
    {
        public long ServiceId { get; set; }
        
        public long ReservationId { get; set; }
        
        public virtual Service Service { get; set; }
        
        public virtual Reservation Reservation { get; set; }
    }
}