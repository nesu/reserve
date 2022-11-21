using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;
using Reserve.Data.Types;

namespace Reserve.Data.Models
{
    [Table("accounts")]
    public class Account : BaseEntity, ITimestamped
    {
        [Required, MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public RoleType Role { get; set; } = RoleType.Client;
        
        [Required, MaxLength(60)]
        public string PasswordHash { get; set; }
        
        [MaxLength(256)]
        public string PhoneNumber { get; set; }
        
        [MaxLength(256)]
        public string GivenName { get; set; }
        
        [MaxLength(256)]
        public string FamilyName { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
        
        public ICollection<Reservation> Reservations { get; set; }
        
        public ICollection<Reservation> AssignedReservations { get; set; }
    }
}