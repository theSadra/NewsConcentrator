using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class MessageMustContain
    {
        [Key]
        public int MMCId { get; set; }
        [Required]
        public string MustContainWord { get; set; }

    }
}
