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




        public void Expirer()
        {
            var now = DateTime.Now;
            
            //Getting expired newses form db.

            foreach (var pNewse in _context.PublishedNewses.ToList())
            {
                int hourpublished = (int) (now - pNewse.DateAdded).TotalHours;


                if (hourpublished > _newshourLifetime)
                {
                    _context.PublishedNewses.RemoveRange(pNewse);
                }

            }

            //Removing expired newses from db.

            _context.SaveChanges();

        }
    }
}
