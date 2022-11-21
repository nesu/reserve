using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Traits;
using Reserve.Data.Types;

namespace Reserve.Data.Models
{
    [Table("global_settings")]
    public class GlobalSetting : BaseEntity, ITimestamped
    {
        public SettingType Type { get; set; }
        
        [MaxLength(256)]
        public string Value { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}