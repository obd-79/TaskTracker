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
    public class UsersController : Controller
    {
        private readonly Proje4Context _context;

        public UsersController(Proje4Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public async Task<IActionResult> CreateAsync()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var projects = _context.Project.Select(p => new { Id = p.Id, Name = p.Name }).ToList();
            var projectSelectListItems = new List<object>
            {
                new { Id = (int?)null, Name = "None" }
             };
            projectSelectListItems.AddRange(projects);
            ViewData["ProjectId"] = new SelectList(projectSelectListItems, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Type,ProjectId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            
            var projects = _context.Project.Select(p => new { Id = p.Id, Name = p.Name }).ToList();
            var projectSelectListItems = new List<object>
    {
        new { Id = (int?)null, Name = "None" }
    };
            projectSelectListItems.AddRange(projects);
            ViewData["ProjectId"] = new SelectList(projectSelectListItems, "Id", "Name", user.ProjectId);

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Type,ProjectId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        public async Task<IActionResult> MyTeam()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var userName = HttpContext.Session.GetString("UserName");
            var manager = await _context.User.FirstOrDefaultAsync(u => u.Name == userName && u.Type == "Manager");

            if (manager == null || manager.ProjectId == null)
            {
                return View("Error"); 
            }

            var teamMembers = _context.User
                              .Where(u => u.ProjectId == manager.ProjectId && u.Type == "Employee")
                              .Select(u => new TeamMemberViewModel
                              {
                                  Id = u.Id,
                                  Name = u.Name,
                                  TotalTasks = u.TotalNumberOfTasks,
                                  CompletedTasks = u.TotalDoneTasks
                              }).ToListAsync();

            return View(await teamMembers);
        }
        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var employee = await _context.User.FindAsync(id);
            if (employee == null)
            {
                return View("Error");
            }

            var taskDetails = await _context.Task
                                    .Where(t => t.UserId == id)
                                    .Select(t => new TaskDetailViewModel
                                    {
                                        Id = t.Id, 
                                        Name = t.Name,
                                        Description = t.Description,
                                        DueDate = t.DueDate,
                                        Completed = t.Completed
                                    }).ToListAsync();

            var viewModel = new EmployeeDetailsViewModel
            {
                Name = employee.Name,
                Tasks = taskDetails
            };

            return View(viewModel);
        }
        private async Task<User> GetCurrentUserAsync()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return null; 
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Name == userName);
            return user;
        }

    }
}
