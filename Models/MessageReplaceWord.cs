using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class MessageReplaceWord
    {
        [Key]
        public int MRWId { get; set; }
        [Required]
        public string Word { get; set; }
        [Required]
        public string ReplaceTo { get; set; }
    }
}
