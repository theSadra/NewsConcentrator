using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class MessageCutAfter
    {
        [Key]
        public int MCAId { get; set; }

        public string CutAfterWord { get; set; }
    }
}
