using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje4.Data;
using Proje4.Models;
using Task = Proje4.Models.Task;

namespace Proje4.Controllers
{
    public class TasksController : Controller
    {
        private readonly Proje4Context _context;

        public TasksController(Proje4Context context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> ViewTask(int id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var task = await _context.Task.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleTaskStatus(int id)
        {
            var task = await _context.Task.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            task.Completed = !task.Completed;
            _context.Update(task);

            if (task.Completed)
            {
                if (task.User != null)
                {
                    task.User.TotalDoneTasks += 1;
                    _context.Update(task.User);
                }
            }
            else
            {
                if (task.User != null && task.User.TotalDoneTasks > 0)
                {
                    task.User.TotalDoneTasks -= 1;
                    _context.Update(task.User);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ViewTask", new { id = task.Id });
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

