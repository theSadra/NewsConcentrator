using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.Controllers
{
    public class TelegramScraperSettingsController : Controller
    {
        private readonly NewsConcentratorDbContext _context;

        public TelegramScraperSettingsController(NewsConcentratorDbContext context)
        {
            _context = context;
        }

        // GET: TelegramScraperSettings
       

        // GET: TelegramScraperSettings/Details/5
       
        // GET: TelegramScraperSettings/Create


        // POST: TelegramScraperSettings/Create
        // GET: TelegramScraperSettings/Edit/5
        public async Task<IActionResult> Edit()
        {
            //var telegramScraperSettings = await _context.Setting.FirstOrDefaultAsync();
            //if (telegramScraperSettings == null)
            //{
            //    return NotFound();
            //}
            return View();
        }

        // POST: TelegramScraperSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,Intervalmins,Deschatid")] TelegramScraperSettings telegramScraperSettings)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telegramScraperSettings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                }
                return RedirectToAction("Index","Home");
            }
            return View(telegramScraperSettings);
        }

        // GET: TelegramScraperSettings/Delete/5
     
        // POST: TelegramScraperSettings/Delete/5
        
    }
}
