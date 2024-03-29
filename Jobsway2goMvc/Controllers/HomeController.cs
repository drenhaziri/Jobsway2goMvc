﻿using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Extensions;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Jobsway2goMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager= userManager;
        }

        public IActionResult Index(int pageSize = 10, int page = 1)
        {
            var result = _context.Jobs.Page(pageSize, page).ToList();
            ViewBag.FeedJobs = result;

            var userid = _userManager.GetUserId(HttpContext.User);

            var users = _context.Users.Where(x=>x.Id != userid).ToList();
            ViewBag.Users = users;
            if (User.Identity.IsAuthenticated)
            {

            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

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
                        var users = from u in _context.Users.Where(s => s.FirstName.Contains(searchString) || s.LastName.Contains(searchString))
                                    select u;
                        return View("~/Views/UserProfile/ShowUser.cshtml", users.ToList());
                    case "Business":
                        var business = from b in _context.Users.Where(s => s.CompanyName.Contains(searchString) || s.CompanyArea.Contains(searchString))
                                       select b;
                        return View("~/Views/UserProfile/ShowUser.cshtml", business.ToList());
                    case "Job":
                        var jobs = from j in _context.Jobs.Where(s => s.Category.Name.Contains(searchString))
                                   select j;
                        return View("~/Views/Jobs/Index.cshtml", jobs.ToList());
                    case "Event":
                        var events = from e in _context.Events.Where(s => s.Title.Contains(searchString) || s.CompanyName.Contains(searchString))
                                     select e;
                        return View("~/Views/Events/Index.cshtml", events.ToList());
                    case "Group":
                        var groups = from g in _context.Groups.Where(s => s.Name.Contains(searchString))
                                     select g;
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

        public IActionResult GetNotifications()
        {
            var notifications = _context.Notifications.ToList();
            return View(notifications);
        }
    }
}