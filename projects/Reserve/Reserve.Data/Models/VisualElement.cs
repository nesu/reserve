using System;
using System.ComponentModel.DataAnnotations.Schema;
using Reserve.Data.Traits;
using Reserve.Data.Types;

namespace Reserve.Data.Models
{
    [Table("visual_elements")]
    public class VisualElement : BaseEntity, ITimestamped
    {
        public ElementType Type { get; set; }
        
        [Column(TypeName = "JSON")]
        public string Properties { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}