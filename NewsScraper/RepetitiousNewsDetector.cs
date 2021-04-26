using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TLSchema;
using NewsConcentratorSystem.Models;
namespace NewsConcentratorSystem.NewsScraper
{
    public static class RepetitiousNewsMediaDetector
    {
        public static bool IsmediaRepetitious(string Mediahash,IQueryable<News> PublishedNewses)
        {
            var result = PublishedNewses.Where(n => n.Mediahash == Mediahash).FirstOrDefault();
            return result != null ? true : false;
        }
    }
}
