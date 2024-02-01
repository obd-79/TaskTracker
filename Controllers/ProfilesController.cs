
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje4.Data;
using Proje4.Models;
using System.Security.Claims;

namespace Proje4.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly Proje4Context _context;
        public ProfilesController(Proje4Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Profile(int id)
        {




            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password
            };

            switch (user.Type)
            {
                case "admin":
                    ViewBag.Layout = "adminLayout";
                    break;
                case "employee":
                    ViewBag.Layout = "employeeLayout";
                    break;
                case "manager":
                    ViewBag.Layout = "managerLayout";
                    break;
                default:
                    ViewBag.Layout = "_Layout";
                    break;

            }
            ViewBag.UserId = user.Id;
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Profile(int id, UserProfileViewModel model)
        {

            var user = await _context.User.FindAsync(id);
            if (ModelState.IsValid)
            {
                if (id != model.Id)
                {
                    return NotFound();
                }


                if (user == null)
                {
                    return NotFound();
                }
                user.Id = model.Id;
                user.Name = model.Name;
                user.Email = model.Email;
                user.Password = model.Password;
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            ViewBag.UserId = user.Id;
            switch (user.Type)
            {
                case "admin":
                    return RedirectToAction("AdminHome", "Home");

                case "employee":
                    return RedirectToAction("EmployeeHome", "Home");

                case "manager":
                    return RedirectToAction("ManagerHome", "Home");

                default:
                    return RedirectToAction("EmployeeHome", "Home");

            }

        }
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
