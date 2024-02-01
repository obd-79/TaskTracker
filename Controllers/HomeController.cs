using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje4.Data;
using Proje4.Models;
using System.Diagnostics;

namespace Proje4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Proje4Context _context;

        public HomeController(ILogger<HomeController> logger, Proje4Context context)
        {
            _logger = logger;
            _context = context;
        }

        
        public async Task<IActionResult> AdminHome()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var user = await _context.User.FirstOrDefaultAsync(u => u.Name == userName);
            var viewModel = new CustomHomeViewModel
            {
                Projects = await _context.Project.Take(4).ToListAsync(),
                Users = await _context.User.Where(u => u.Type != "admin").Take(4).ToListAsync()

            };

            ViewBag.UserId = user.Id;
            return View(viewModel);
        }
        public async Task<IActionResult> ManagerHome()
        {
            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;
            var userName = HttpContext.Session.GetString("UserName");
            var manager = await _context.User.FirstOrDefaultAsync(u => u.Name == userName && u.Type == "manager");

            if (manager == null || manager.ProjectId == null)
            {
                
                return View("Error");
            }

            var currentProject = await _context.Project
                                               .Include(p => p.Tasks)
                                               .FirstOrDefaultAsync(p => p.UserId == manager.Id);

            if (currentProject == null)
            {
               
                return View("Error");
            }

            var employees = await _context.User
                                          .Where(u => u.ProjectId == currentProject.Id && u.Type == "employee")
                                          .ToListAsync();

            
            var viewModel = new ManagerHomeViewModel
            {
                CurrentProject = currentProject,
                Employees = employees
            };

            return View(viewModel);
        }

        public async Task<IActionResult> EmployeeHome()
        {


            var user1 = await GetCurrentUserAsync();
            ViewBag.UserId = user1.Id;


            var tasks = await _context.Task.Where(t => t.UserId == user1.Id).ToListAsync();
            var viewModel = new CustomHomeViewModel
            {
                Tasks = tasks,

            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }


        public async Task<IActionResult> Search(string query)
        {

            var user = await GetCurrentUserAsync();
            var combinedResults = new CustomHomeViewModel();
            ViewBag.UserId = user.Id;
            combinedResults.searchQuery = query;
            int.TryParse(query, out int intQuery);
            if (user.Type == "admin")
            {
                ViewBag.Layout = "adminLayout";

                var usersResults = await _context.User.Include(u => u.Project).Where(u => (u.Name.Contains(query) || u.Id == intQuery) && u.Type != "admin").ToListAsync();
                var projectResults = await _context.Project.Include(p => p.User).Where(p => p.Name.Contains(query) || p.Id == intQuery).ToListAsync();




                combinedResults.Users = usersResults;
                combinedResults.Projects = projectResults;
                return View("AdminSearchResults", combinedResults);

            }
            else if (user.Type == "manager")
            {
                ViewBag.Layout = "managerLayout";

                var taskResults = await _context.Task.Include(t => t.User).Where(t => t.ProjectId == user.ProjectId && (t.Name.Contains(query) || t.Id == intQuery)).ToListAsync();
                var employeeResults = await _context.User.Where(u => u.ProjectId == user.ProjectId && (u.Name.Contains(query) || u.Id == intQuery) && u.Type == "employee").ToListAsync();
                var messageResults = await _context.Message.Include(m => m.User).Where(m => m.Text.Contains(query) || m.User.Name.Contains(query)).ToListAsync();



                combinedResults.Tasks = taskResults;
                combinedResults.Users = employeeResults;
                combinedResults.Messages = messageResults;
                return View("ManagerSearchResults", combinedResults);


            }
            else if (user.Type == "employee")
            {
                var taskResults = await _context.Task.Include(t => t.User).Where(t => t.ProjectId == user.ProjectId && t.UserId == user.Id &&(t.Name.Contains(query) || t.Id == intQuery)).ToListAsync();
                var messageResults = await _context.Message.Include(m => m.User).Where(m => m.Text.Contains(query) || m.User.Name.Contains(query)).ToListAsync();
                ViewBag.Layout = "employeeLayout";





                combinedResults.Tasks = taskResults;
                combinedResults.Messages = messageResults;
                return View("EmployeeSearchResults", combinedResults);

            }
            /*
             switch (user.Type)
            {
 
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
             */

            return View("SearchResults", combinedResults);

        }
        [HttpGet]
      
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

