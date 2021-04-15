using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class Settings
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "متن شروع")]
        public string StartDescription { get; set; }
        [Display(Name = "متن پایان")]
        public string EndDescription { get; set; }
    }
}
