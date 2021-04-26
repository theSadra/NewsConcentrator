using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.NewsScraper
{
    public class PublishedNewsExpirer
    {
        private int _newshourLifetime;
        private NewsConcentratorDbContext _context;
        public PublishedNewsExpirer( int newshaourlifetime, NewsConcentratorDbContext context)
        {
            _newshourLifetime = newshaourlifetime;
            _context = context;
        }




        public void Expirer(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            
            //Getting expired newses form db.
            var expired = _context.PublishedNewses.AsNoTracking().Where(n => (int) (now - n.DateAdded).TotalHours > _newshourLifetime);

            //Removing expired newses from db.

            _context.PublishedNewses.RemoveRange(expired);
            _context.SaveChanges();

        }
    }
}
