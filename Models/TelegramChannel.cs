using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class TelegramChannel  
    {
        [Key]
        public int ChannelId { get; set; }
        [Required]
        [Display(Name = "یوزر نیم کانال")]
        public string ChannelUserName { get; set; }

        [Display(Name = "فیلتر کلمه اجباری")]
        public bool HasContainFilter { get; set; }
        [Display(Name = "فیلتر جایگذاری کلمه")]
        public bool HasReplaceWordFilter { get; set; }
        [Display(Name = "فیلتر بریدن کلمه")]
        public bool HasCutAfterFilter { get; set; }

        public int IntervalMins { get; set; }

        [Display(Name = "وضعیت فعالیت")]
        public bool ActivityStatus { get; set; }

            
        //Navigation Properties
        [Display(Name = "کلمه های اجباری")]
        public ICollection<MessageMustContain> MustContainWords { get; set; }
        [Display(Name = "کلمه های جایگذین شونده")]
        public ICollection<MessageReplaceWord> ReplaceWords { get; set; }
        [Display(Name = "کلمه های تمام کننده پیام")]
        public ICollection<MessageCutAfter> CutAfterWords { get; set; }

    }
}
