using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsConcentratorSystem.Models;

namespace NewsConcentratorSystem.Controllers
{
    public class SettingsController : Controller
    {
        private readonly NewsConcentratorDbContext _context;

        public SettingsController(NewsConcentratorDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var settings = _context.Settings.FirstOrDefault();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Index(Settings settings)
        {
            var entity = _context.Settings.FirstOrDefault();
            entity.StartDescription = settings.StartDescription;
            entity.EndDescription = settings.EndDescription;
            _context.SaveChanges();
            return View();
        }
    }
}
