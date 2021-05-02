using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.NewsScraper;

namespace NewsConcentratorSystem.Controllers
{
    public class TelegramChannelsController : Controller
    {
        private readonly NewsConcentratorDbContext _context;

        public TelegramChannelsController(NewsConcentratorDbContext context)
        {
            _context = context;
        }

        // GET: TelegramChannels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Channels.ToListAsync());
        }

        // GET: TelegramChannels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramChannel = await _context.Channels
                .FirstOrDefaultAsync(m => m.ChannelId == id);
            if (telegramChannel == null)
            {
                return NotFound();
            }

            return View(telegramChannel);
        }

        // GET: TelegramChannels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TelegramChannels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChannelId,ChannelUserName,HasContainFilter,IntervalMins")] TelegramChannel telegramChannel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TelegramCollector.mustwait = true;
                    TelegramCollector.mustwaittilme = 15000;
                    Thread.Sleep(5000);
                    var channel = TelegramClientManager.GetTChannelbyUsername(telegramChannel.ChannelUserName);
                    if (channel == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    Thread.Sleep(8000);
                    NewsConcentratorSystem.NewsScraper.TelegramClientManager
                        .JoinChannel(telegramChannel.ChannelUserName)
                        .Wait();
                    //Insert Entity in DB
                    telegramChannel.ChannelChatID = channel.Id.ToString();
                    telegramChannel.AccessHash = channel.AccessHash.ToString();
                    telegramChannel.ChannelUserName = channel.Username;
                    telegramChannel.ChannelTitle = channel.Title;
                    _context.Add(telegramChannel);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return NotFound();
                }
            }
            return View(telegramChannel);
        }

        // GET: TelegramChannels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramChannel = await _context.Channels.FindAsync(id);
            _context.Entry(telegramChannel).Collection(c => c.ReplaceWords).Load();
            _context.Entry(telegramChannel).Collection(c => c.MustContainWords).Load();
            _context.Entry(telegramChannel).Collection(c => c.MustnotContainFilters).Load();

            if (telegramChannel == null)
            {
                return NotFound();
            }
            
            return View(telegramChannel);

        }

        // POST: TelegramChannels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChannelId,IntervalMins")] TelegramChannel telegramChannel)
        {
            if (id != telegramChannel.ChannelId)
            {
                return NotFound();
            }

            
                try
                {
                 var channel =  _context.Channels.Where(c => c.ChannelId == telegramChannel.ChannelId).FirstOrDefault();




                    _context.Update(channel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelegramChannelExists(telegramChannel.ChannelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = telegramChannel.ChannelId });
        }

        // GET: TelegramChannels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramChannel = await _context.Channels
                .FirstOrDefaultAsync(m => m.ChannelId == id);
            if (telegramChannel == null)
            {
                return NotFound();
            }

            return View(telegramChannel);
        }

        // POST: TelegramChannels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var telegramChannel = await _context.Channels.FindAsync(id);



            //Cascade delete
            _context.Entry(telegramChannel).Collection(c => c.ReplaceWords).Load();
            _context.Entry(telegramChannel).Collection(c => c.MustContainWords).Load();
            _context.Entry(telegramChannel).Collection(c => c.MustnotContainFilters).Load();



            if (telegramChannel.ReplaceWords!=null)
                _context.RemoveRange(telegramChannel.ReplaceWords);
            if (telegramChannel.MustContainWords != null)
                _context.RemoveRange(telegramChannel.MustContainWords);
            if (telegramChannel.MustnotContainFilters != null)
                _context.RemoveRange(telegramChannel.MustnotContainFilters);
            _context.Remove(telegramChannel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TelegramChannelExists(int id)
        {
            return _context.Channels.Any(e => e.ChannelId == id);
        }
    }
}
