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
    public class MessagesController : Controller
    {
        private readonly Proje4Context _context;

        public MessagesController(Proje4Context context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            var proje4Context = _context.Message.Include(m => m.User);
            return View(await proje4Context.ToListAsync());
        }
        public async Task<IActionResult> AdminIndex()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            var proje4Context = _context.Message.Include(m => m.User);
            return View(await proje4Context.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public async Task<IActionResult> CreateAsync()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            ViewData["UsersIds"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,UserId")] Message message)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            if (ModelState.IsValid)
            {
                message.User = user1;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["UsersIds"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,UserId")] Message message)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    message.User = user1;
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            ViewBag.Layout = user1.Type + "Layout";
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Message.FindAsync(id);
            if (message != null)
            {
                _context.Message.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.Id == id);
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
