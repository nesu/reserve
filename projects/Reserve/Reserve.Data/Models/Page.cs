using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table("pages")]
    public class Page : BaseEntity, ITimestamped
    {
        public string Title { get; set; }
        
        [MaxLength(256)]
        public string Identifier { get; set; }

        [Column(TypeName = "TEXT")]
        public string Contents { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}