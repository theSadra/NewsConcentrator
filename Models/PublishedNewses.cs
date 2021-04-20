using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    public class PublishedNewses
    {
        [Key]
        public int id { get; set; }
        [AllowNull]
        public string Mediahash { get; set; }
        [NotNull]
        public string TextMessage { get; set; }
    }
}
