using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reserve.Data.Traits;

namespace Reserve.Data.Models
{
    [Table("service_categories")]
    public class ServiceCategory : BaseEntity, ITimestamped
    {
        [MaxLength(256)]
        public string Label { get; set; }
        
        public long? ParentCategoryId { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "TIMESTAMP")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
        
        public ServiceCategory ParentCategory { get; set; }
        
        public ICollection<ServiceCategory> ChildCategories { get; set; }
    }
}