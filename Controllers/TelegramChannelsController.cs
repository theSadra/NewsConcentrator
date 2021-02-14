using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;
using NewsConcentratorSystem.Models;

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
                _context.Add(telegramChannel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("ChannelId,ChannelUserName,HasContainFilter,IntervalMins")] TelegramChannel telegramChannel)
        {
            if (id != telegramChannel.ChannelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telegramChannel);
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
                return RedirectToAction(nameof(Index));
            }
            return View(telegramChannel);
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
            _context.Channels.Remove(telegramChannel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelegramChannelExists(int id)
        {
            return _context.Channels.Any(e => e.ChannelId == id);
        }
    }
}
