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
    public class MessageReplaceWordsController : Controller
    {
        private readonly NewsConcentratorDbContext _context;

        public MessageReplaceWordsController(NewsConcentratorDbContext context)
        {
            _context = context;
        }

        // GET: MessageReplaceWords
        public async Task<IActionResult> Index()
        {
            return View(await _context.MessageReplaceWords.ToListAsync());
        }

        // GET: MessageReplaceWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageReplaceWord = await _context.MessageReplaceWords
                .FirstOrDefaultAsync(m => m.MRWId == id);
            if (messageReplaceWord == null)
            {
                return NotFound();
            }

            return View(messageReplaceWord);
        }

        // GET: MessageReplaceWords/Create
        public IActionResult Create(int id)
        {
            ViewBag.id = id;
            HttpContext.Session.SetString("id",id.ToString());
            return View();
        }

        // POST: MessageReplaceWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MRWId,Word,ReplaceTo")] MessageReplaceWord messageReplaceWord)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(HttpContext.Session.GetString("id"));
                 var channel =  _context.Channels.Where(c => c.ChannelId == id).FirstOrDefault();
                 _context.Entry(channel).Collection(c=>c.ReplaceWords).Load();
                channel.ReplaceWords.Add(messageReplaceWord);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "TelegramChannels", new { id = id });
            }
            return BadRequest();
        }

        // GET: MessageReplaceWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageReplaceWord = await _context.MessageReplaceWords.FindAsync(id);
            if (messageReplaceWord == null)
            {
                return NotFound();
            }
            return View(messageReplaceWord);
        }

        // POST: MessageReplaceWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MRWId,Word,ReplaceTo")] MessageReplaceWord messageReplaceWord)
        {
            if (id != messageReplaceWord.MRWId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageReplaceWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageReplaceWordExists(messageReplaceWord.MRWId))
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
            return View(messageReplaceWord);
        }

        // GET: MessageReplaceWords/Delete/5
        public async Task<IActionResult> Delete(int channelid,int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageReplaceWord = await _context.MessageReplaceWords.FindAsync(id);
            _context.MessageReplaceWords.Remove(messageReplaceWord);
            await _context.SaveChangesAsync();

           return RedirectToAction("Edit", "TelegramChannels", new { id = channelid });
        }

        // POST: MessageReplaceWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageReplaceWord = await _context.MessageReplaceWords.FindAsync(id);
            _context.MessageReplaceWords.Remove(messageReplaceWord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageReplaceWordExists(int id)
        {
            return _context.MessageReplaceWords.Any(e => e.MRWId == id);
        }
    }
}
