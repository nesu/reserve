using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Reserve.Data.Types;

namespace Reserve.Data.Models
{
    [Table("account_options")]
    public class AccountOption : BaseEntity
    {
        public long AccountId { get; set; }   
        
        public AccountOptionType Type { get; set; }
        
        [MaxLength(256)]
        public string Value { get; set; }
        
        public virtual Account Account { get; set; }
    }
}