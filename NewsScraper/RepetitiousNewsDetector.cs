using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TLSchema;
using NewsConcentratorSystem.Models;
namespace NewsConcentratorSystem.NewsScraper
{
    public static class RepetitiousNewsDetector
    {
        public static bool IsmediaRepetitious(string Mediahash,DbSet<News> PublishedNewses)
        {
            var result = PublishedNewses.Where(n => n.Mediahash == Mediahash).FirstOrDefault();
            return result != null ? true : false;
        }
    }
}
