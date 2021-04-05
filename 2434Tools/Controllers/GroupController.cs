using _2434Tools.Data;
using _2434Tools.Models;
using _2434Tools.Models.ViewModels;
using _2434Tools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Controllers
{
    public class GroupController : Controller
    {
        #region Declarations
        readonly ApplicationDbContext _context;
        readonly IUserPermissionsService _permissions;
        public GroupController(ApplicationDbContext context, IUserPermissionsService permissions)
        {
            this._context = context;
            this._permissions = permissions;
        }
        #endregion
        #region Index
        public async Task<IActionResult> Index()
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Groups = await _context.Groups.ToListAsync();
            ViewBag.Groups = Groups;
            return this.View();
        }
        #endregion
        #region Create
        public IActionResult Create()
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            return this.View(new GroupViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupViewModel model)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            if (ModelState.IsValid)
            {
                var Group = new Group()
                {
                    Name = model.Name,
                };
                _context.Add(Group);
                await _context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            return this.View(model);
        }
        #endregion
        #region Edit
        public async Task<IActionResult> Edit(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Group = await _context.Groups.SingleOrDefaultAsync(_group => _group.Id == id);
            if (Group == null) return this.NotFound();
            ViewBag.id = id;
            return this.View(new GroupViewModel() { Name = Group.Name });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32 id, GroupViewModel model)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Group = await _context.Groups.SingleOrDefaultAsync(_group => _group.Id == id);
            if (Group == null) return this.NotFound();
            if (ModelState.IsValid)
            {
                Group.Name = model.Name;
                await _context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            ViewBag.id = id;
            return this.View(model);
        }
        #endregion
        #region Delete
        public async Task<IActionResult> Delete(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Group = await _context.Groups.SingleOrDefaultAsync(_group => _group.Id == id);
            if (Group == null) return this.NotFound();
            ViewBag.Group = Group;
            return this.View();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(Int32 id)
        {
            if (!_permissions.IsAdmin()) return this.NotFound();
            var Group = await _context.Groups.SingleOrDefaultAsync(_group => _group.Id == id);
            if (Group == null) return this.NotFound();
            _context.Remove(Group);
            await _context.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }
        #endregion
    }
}
