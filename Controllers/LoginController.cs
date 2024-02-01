using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje4.Data;
using Proje4.Models;

namespace Proje4.Controllers
{
    public class LoginController : Controller
    {
        private readonly Proje4Context _context;
        public LoginController(Proje4Context context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authenticate(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.User
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && user.Password == model.Password) 
                {
                    HttpContext.Session.SetString("UserName", user.Name);

                    if (user.Type == "admin")
                    {
                        return RedirectToAction("AdminHome", "Home");
                    }
                    else if (user.Type == "manager") 
                    {
                        return RedirectToAction("ManagerHome", "Home");
                    }
                    else if (user.Type == "employee")
                    {
                        return RedirectToAction("EmployeeHome", "Home");
                    }
                   
                }
            }

            ModelState.AddModelError("", "Email and Password do not match");
            ViewBag.ErrorMessage = "Invalid credentials. Please try again.";
            return View("Login");
        }

    }
}
