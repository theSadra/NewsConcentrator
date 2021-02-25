using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class TelegramScraperSettings
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "تایم واکشی بر دقیقه")]
        public int Intervalmins { get; set; }
        [Required]
        [Display(Name = "چت آیدی کانال مقصد")]
        public string Deschatid { get; set; }
    }
}
