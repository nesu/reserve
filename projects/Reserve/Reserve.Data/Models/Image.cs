using System;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table(("images"))]
    public class Image : BaseEntity, ITimestamped
    {
        public long ServiceId { get; set; }
        
        public string ImagePath { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
        
        public virtual Service Service { get; set; } 
    }
}