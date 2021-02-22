using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.Controllers
{
    public class MessageMustContainsController : Controller
    {
        private readonly NewsConcentratorDbContext _context;

        public MessageMustContainsController(NewsConcentratorDbContext context)
        {
            _context = context;
        }

        // GET: MessageMustContains
        public async Task<IActionResult> Index()
        {
            return View(await _context.MessageMustContains.ToListAsync());
        }

        // GET: MessageMustContains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageMustContain = await _context.MessageMustContains
                .FirstOrDefaultAsync(m => m.MMCId == id);
            if (messageMustContain == null)
            {
                return NotFound();
            }

            return View(messageMustContain);
        }

        // GET: MessageMustContains/Create
        public IActionResult Create(int id)
        {
            HttpContext.Session.SetString("id", id.ToString());
            return View();
        }

        // POST: MessageMustContains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MMCId,MustContainWord")] MessageMustContain messageMustContain)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(HttpContext.Session.GetString("id"));
                var channel = _context.Channels.Where(c => c.ChannelId == id).FirstOrDefault();
                _context.Entry(channel).Collection(c => c.MustContainWords).Load();
                channel.MustContainWords.Add(messageMustContain);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "TelegramChannels", new {id = id});
            }
            return View(messageMustContain);
        }

        // GET: MessageMustContains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageMustContain = await _context.MessageMustContains.FindAsync(id);
            if (messageMustContain == null)
            {
                return NotFound();
            }
            return View(messageMustContain);
        }

        // POST: MessageMustContains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MMCId,MustContainWord")] MessageMustContain messageMustContain)
        {
            if (id != messageMustContain.MMCId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageMustContain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageMustContainExists(messageMustContain.MMCId))
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
            return View(messageMustContain);
        }

        // GET: MessageMustContains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageMustContain = await _context.MessageMustContains
                .FirstOrDefaultAsync(m => m.MMCId == id);
            if (messageMustContain == null)
            {
                return NotFound();
            }

            return View(messageMustContain);
        }

        // POST: MessageMustContains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageMustContain = await _context.MessageMustContains.FindAsync(id);
            _context.MessageMustContains.Remove(messageMustContain);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageMustContainExists(int id)
        {
            return _context.MessageMustContains.Any(e => e.MMCId == id);
        }
    }
}
