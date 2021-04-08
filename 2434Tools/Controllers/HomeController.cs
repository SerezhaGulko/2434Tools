using _2434Tools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using _2434Tools.Data;
using Microsoft.EntityFrameworkCore;

namespace _2434Tools.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            DateTime since = DateTime.UtcNow - new TimeSpan(12, 0, 0);
            var allVideos = await _context.Videos.Where(_v =>
                _v.Status == VideoStatus.Live
                || _v.Status == VideoStatus.Upcoming
                || (_v.Status == VideoStatus.Finished && _v.Published > since)
                )
                .Include(v => v.Creator)
                .ToListAsync();
            ViewBag.Live = allVideos.Where(v => v.Status == VideoStatus.Live).OrderByDescending(v => v.LiveStartTime).ToList();
            ViewBag.Upcoming = allVideos.Where(v => v.Status == VideoStatus.Upcoming).OrderBy(v => v.LiveStartTime).ToList();
            ViewBag.New = allVideos.Where(v => v.Status == VideoStatus.Finished).OrderByDescending(v => v.Published).ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
