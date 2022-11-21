using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserve.Data.Models
{
    [Table("services")]
    public class Service : BaseEntity
    {
        [MaxLength(256)]
        public string Label { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        
        public double Price { get; set; }
        
        public int Duration { get; set; }
        
        public long CategoryId { get; set; }
        
        public ServiceCategory Category { get; set; }
        
        public ICollection<ServiceAssignee> Assignees { get; set; }
        
        public ICollection<Image> Images { get; set; }
    }
}