using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Proje4.Data;
using Proje4.Models;
using System.Security.Claims;

namespace Proje4.Controllers
{
    public class ManagerTasksController : Controller
    {
        private readonly Proje4Context _context;

        public ManagerTasksController(Proje4Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var task = await _context.Task
                                     .Include(t => t.User)
                                     .FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return View("Error");
            }

            var viewModel = new TaskDetailViewModel
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Completed = task.Completed,
                AssignedToUserName = task.User?.Name 
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TaskDetailViewModel model)
        {
           
                var task = await _context.Task.FindAsync(model.Id);
                if (task != null)
                {
                    task.DueDate = model.DueDate;
                    task.Description = model.Description;
                    // Do not update Name, Completed, or AssignedToUserName as they are not editable
                    await _context.SaveChangesAsync();
                    return RedirectToAction("MyTeam", "Users");
                }

               
            
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var task = await _context.Task.FindAsync(id);
            var assignedUser = await _context.User.FindAsync(task.UserId);
            if (task != null)
            {
                
                _context.Task.Remove(task);
                await _context.SaveChangesAsync();
            }
            
            if (assignedUser != null)
            {
                assignedUser.TotalNumberOfTasks -= 1;
                _context.Update(assignedUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "ManagerTasks");
        }
        public async Task<IActionResult> Index()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var userName = HttpContext.Session.GetString("UserName");
            var manager = await _context.User.FirstOrDefaultAsync(u => u.Name == userName && u.Type == "Manager");

            if (manager == null || manager.ProjectId == null)
            {
                return View("Error");
            }

            var tasks = await _context.Task
                                      .Where(t => t.ProjectId == manager.ProjectId)
                                      .ToListAsync();


            return View(tasks);
        }
        public async Task<IActionResult> Create()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var currentUserName = HttpContext.Session.GetString("UserName");
            var manager = await _context.User.FirstOrDefaultAsync(u => u.Name == currentUserName);

            if (manager == null || manager.ProjectId == null)
            {
                
                return View("Error");
            }

            var employees = await _context.User
                                          .Where(u => u.ProjectId == manager.ProjectId && u.Type == "Employee")
                                          .ToListAsync();

            var viewModel = new CreateTaskViewModel
            {
                Employees = employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            
                
                var currentUserName = HttpContext.Session.GetString("UserName");
                var manager = await _context.User.FirstOrDefaultAsync(u => u.Name == currentUserName);

               
                if (manager == null || manager.ProjectId == null)
                {
                    
                    ModelState.AddModelError("", "Manager or Project not found.");
                    return View(model);
                }

                var newTask = new Models.Task
                {
                    Name = model.Name,
                    DueDate = model.DueDate,
                    UserId = model.AssignedToUserId,
                    Description = model.Description,
                    ProjectId = manager.ProjectId,
                    Completed = false
                };

                _context.Task.Add(newTask);
                await _context.SaveChangesAsync();
            var assignedUser = await _context.User.FindAsync(model.AssignedToUserId);
            if (assignedUser != null)
            {
                assignedUser.TotalNumberOfTasks += 1;
                _context.Update(assignedUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
            

    
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
