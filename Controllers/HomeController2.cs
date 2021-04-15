using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Emit;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.Controllers
{
    public class MustnotContainController : Controller
    {

        private NewsConcentratorDbContext _context { get; set; }
        public MustnotContainController(NewsConcentratorDbContext context)
        {
            _context = context;
        }


        public IActionResult Create(int id)
        {
            ViewBag.id = id;
            HttpContext.Session.SetString("id", id.ToString());
            return View();
        }


        // POST: HomeController2/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MessageMustnotContainFilter entity)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(HttpContext.Session.GetString("id"));
                var channel = _context.Channels.Where(c => c.ChannelId == id).FirstOrDefault();
                _context.Entry(channel).Collection(c => c.MustnotContainFilters).Load();
                channel.MustnotContainFilters.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "TelegramChannels", new { id = id });
            }
            return View(entity);
        }


        // GET: HomeController2/Delete/5
        public async Task<IActionResult> Delete(int channelid, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageMustnotContain = await _context.MessageMustnotContain.FindAsync(id);
            _context.Remove(messageMustnotContain);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "TelegramChannels", new { id = channelid });
        }
        // POST: HomeController2/Delete/5
       
    }
}
