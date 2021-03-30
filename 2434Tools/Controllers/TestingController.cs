using _2434Tools.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Controllers
{
    public class TestingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TestingController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Videos()
        {
            var videos = await _context.Videos.ToListAsync();
            ViewBag.videos = videos;
            return this.View();
        }

        public async Task<IActionResult> Livers()
        {
            var livers = await _context.Livers.ToListAsync();
            ViewBag.livers = livers;
            return this.View();
        }
    }
}
