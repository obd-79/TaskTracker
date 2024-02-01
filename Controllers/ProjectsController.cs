using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje4.Data;
using Proje4.Models;

namespace Proje4.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly Proje4Context _context;

        public ProjectsController(Proje4Context context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var proje4Context = _context.Project.Include(p => p.User);
            return View(await proje4Context.ToListAsync());
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> CreateAsync()
        {
            /*
            var managerUsers = _context.User.Where(u => u.Type == "manager");
            ViewData["UserId"] = new SelectList(managerUsers, "Id", "Name");
            return View();
            */
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var managerUsers = _context.User
                               .Where(u => u.Type == "manager" && !_context.Project.Any(p => p.UserId == u.Id))
                               .Select(u => new { Id = (int?)u.Id, Name = u.Name })
                               .ToList();

            var managerSelectListItems = new List<object>
            {
            new { Id = (int?)null, Name = "None" }
              };
            managerSelectListItems.AddRange(managerUsers);

            var managerSelectList = new SelectList(managerSelectListItems, "Id", "Name");

            ViewData["UsersIds"] = managerSelectList;
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UserId,DueDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                if (project.UserId != null)
                {
                    var managerUser = await _context.User.FindAsync(project.UserId);
                    if (managerUser != null)
                    {
                        
                        managerUser.ProjectId = project.Id;
                        _context.Update(managerUser);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", project.UserId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var managerUsers = _context.User
                               .Where(u => u.Type == "manager")
                               .Select(u => new { Id = (int?)u.Id, Name = u.Name })
                               .ToList();

            var managerSelectListItems = new List<object>
            {
            new { Id = (int?)null, Name = "" }
              };
            managerSelectListItems.AddRange(managerUsers);

            var managerSelectList = new SelectList(managerSelectListItems, "Id", "Name",project.UserId );

            ViewData["UsersIds"] = managerSelectList;
            return View(project);
            /* var managerUsers = _context.User.Where(u => u.Type == "manager");
            ViewData["UserId"] = new SelectList(managerUsers, "Id", "Name", project.UserId);
            return View(project);
            */
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UserId,DueDate")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    if (project.UserId.HasValue)
                    {
                        // Find the user and update their ProjectId
                        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == project.UserId.Value);
                        if (user != null)
                        {
                            user.ProjectId = project.Id;
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", project.UserId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project != null)
            {
                _context.Project.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
        private async Task<User> GetCurrentUserAsync()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return null; // or handle appropriately if a user must be logged in
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Name == userName);
            return user;
        }
    }
}
