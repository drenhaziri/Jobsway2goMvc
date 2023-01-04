using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Jobsway2goMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchString, SearchEnum searchEnum)
        {
            string model = searchEnum.ToString();

            if (!String.IsNullOrEmpty(searchString))
            {

                switch (model)
                {
                    case "User":
                        var users = from m in _context.Users
                                    select m;
                        users = users.Where(s => s.FirstName.Contains(searchString) || s.LastName.Contains(searchString));
                        return View("~/Views/UserProfile/ShowUser.cshtml", users.ToList());
                    case "Business":
                        var business = from m in _context.Users
                                       select m;
                        business = business.Where(s => s.CompanyName.Contains(searchString));
                        return View("~/Views/UserProfile/ShowUser.cshtml", business.ToList());
                    case "Job":
                        var jobs = from m in _context.Jobs
                                   select m;
                        jobs = jobs.Where(s => s.Category.Name.Contains(searchString));
                        return View("~/Views/Jobs/Index.cshtml", jobs.ToList());
                    case "Event":
                        var events = from m in _context.Events
                                     select m;
                        events = events.Where(s => s.Title.Contains(searchString) || s.CompanyName.Contains(searchString));
                        return View("~/Views/Events/Index.cshtml", events.ToList());
                    case "Group":
                        var groups = from m in _context.Groups
                                     select m;
                        groups = groups.Where(s => s.Name.Contains(searchString));
                        return View("~/Views/Groups/Index.cshtml", groups.ToList());
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}