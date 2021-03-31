using _2434Tools.Data;
using _2434Tools.Models;
using _2434Tools.Models.ViewModels;
using _2434Tools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Controllers
{
    public class LiverController : Controller
    {
        #region Declarations
        readonly ApplicationDbContext _context;
        readonly IUserPermissionsService _permissions;
        public LiverController(ApplicationDbContext context,IUserPermissionsService permissions)
        {
            this._context = context;
            this._permissions = permissions;
        }
        #endregion
        #region Index
        public async Task<IActionResult> Index(Int32? id)
        {
            if (id != null) return this.RedirectToAction("Details", new { id });
            var Livers = await _context.Livers.ToListAsync();
            ViewBag.Livers = Livers;
            return this.View();
        }
        // Details
        public async Task<IActionResult> Details(Int32 id)
        {
            var Liver = await _context.Livers.SingleOrDefaultAsync(_liver => _liver.Id == id);
            ViewBag.Liver = Liver;
            return this.View("Details");
        }
        #endregion
        #region Create
        public async Task<IActionResult> Create()
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            ViewBag.Groups = new SelectList(await _context.Groups.ToListAsync(), "Id", "Name");
            return this.View(new LiverViewModel() { Graduated = false });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiverViewModel model)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            if (ModelState.IsValid)
            {
                var Liver = new Liver()
                {
                    ChannelId   = model.ChannelId,
                    Name        = model.Name,
                    Graduated   = model.Graduated,
                    GroupId     = model.GroupId,
                    TwitterLink = model.TwitterLink
                };
                _context.Add(Liver);
                await _context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            ViewBag.Groups = new SelectList(await _context.Groups.ToListAsync(), "Id", "Name", model.GroupId);
            return this.View(model);
        }
        #endregion
        #region Edit
        public async Task<IActionResult> Edit(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Liver = await _context.Livers.SingleOrDefaultAsync(_liver => _liver.Id == id);
            if (Liver == null) return this.NotFound();
            var Groups = await _context.Groups.ToListAsync();
            ViewBag.Groups = new SelectList(Groups, "Id", "Name", Groups.Where(_group => _group.Id == Liver.GroupId));
            return this.View(new LiverViewModel()
            { Graduated = Liver.Graduated, GroupId = Liver.GroupId, Name = Liver.Name, TwitterLink = Liver.TwitterLink,ChannelId = Liver.ChannelId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32 id, LiverViewModel model)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Liver = await _context.Livers.SingleOrDefaultAsync(_liver => _liver.Id == id);
            if (Liver == null) return this.NotFound();
            if (ModelState.IsValid)
            {
                Liver.GroupId = model.GroupId;
                Liver.Name = model.Name;
                Liver.Graduated = model.Graduated;
                Liver.TwitterLink = model.TwitterLink;
                await _context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            ViewBag.Groups = new SelectList(await _context.Groups.ToListAsync(), "Id", "Name", model.GroupId);
            return this.View(model);
        }
        #endregion
        #region Delete
        public async Task<IActionResult> Delete(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Liver = await _context.Livers.SingleOrDefaultAsync(_liver => _liver.Id == id);
            if (Liver == null) return this.NotFound();
            ViewBag.Liver = Liver;
            return this.View();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Liver = await _context.Livers.SingleOrDefaultAsync(_liver => _liver.Id == id);
            if (Liver == null) return this.NotFound();
            _context.Remove(Liver);
            await _context.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }
        #endregion
    }
}
