using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TLSchema;

namespace NewsConcentratorSystem.Models
{
    public class News
    {
        [Key]
        public int id { get; set; }
        [AllowNull]
        public string Mediahash { get; set; }
        [AllowNull]
        public string TextMessage { get; set; }
        public DateTime DateAdded { get; set; }





      

    } 
    

}
