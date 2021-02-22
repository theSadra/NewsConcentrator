using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.Models
{
    public class NewsConcentratorDbContext : DbContext
    {
        public NewsConcentratorDbContext(DbContextOptions options) :base(options)
        {
            
        }

        public NewsConcentratorDbContext()
        {
            
        }
        public DbSet<TelegramChannel> Channels { set; get; }
        public DbSet<MessageMustContain> MessageMustContains { get; set; }
        public DbSet<MessageReplaceWord> MessageReplaceWords { get; set; }
        public DbSet<MessageCutAfter> MessageCutAfters { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = newsScraper.db");
        }
    }
}
