using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Reports.ToListAsync());
        }

        public async Task<IActionResult> ReportJob(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportJob(Report report, Job job, ReportEnum reportEnum)
        {
            var jobRef = await _context.Jobs.FirstOrDefaultAsync(s => s.Id == job.Id);
            string model = reportEnum.ToString();
            ModelState.Remove("JobId");
            ModelState.Remove("PostId");
            ModelState.Remove("reportType");
            ModelState.Remove("GroupId");

            report.Id = 0;
            report.JobId = jobRef.Id;
            report.PostId = null;
            report.reportType = reportEnum;
            report.GroupId = null;
            _context.Add(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ReportPost(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportPost(Report report, Post post, ReportEnum reportEnum)
        {
            var postRef = await _context.Posts.FirstOrDefaultAsync(s => s.Id == post.Id);
            string model = reportEnum.ToString();
            ModelState.Remove("JobId");
            ModelState.Remove("PostId");
            ModelState.Remove("reportType");
            ModelState.Remove("GroupId");

            report.Id = 0;
            report.JobId = null;
            report.PostId = postRef.Id;
            report.reportType = reportEnum;
            report.GroupId = null;
            _context.Add(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ReportGroup(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportGroup(Report report, Group group, ReportEnum reportEnum)
        {
            var groupRef = await _context.Groups.FirstOrDefaultAsync(s => s.Id == group.Id);
            string model = reportEnum.ToString();
            ModelState.Remove("JobId");
            ModelState.Remove("PostId");
            ModelState.Remove("reportType");
            ModelState.Remove("GroupId");

            report.Id = 0;
            report.JobId = null;
            report.PostId = null;
            report.GroupId = groupRef.Id;
            report.reportType = reportEnum;
            _context.Add(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int? id)
        {
            ReportViewModel reportView = new ReportViewModel();

            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }
            reportView.Report = report;

            if (report.JobId != null)
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == report.JobId);
                reportView.Job = job;
                return View(reportView);
            }
            else if (report.PostId != null)
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == report.PostId);
                reportView.Post = post;
                return View(reportView);
            }
            else if (report.GroupId != null)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(p => p.Id == report.GroupId);
                reportView.Group = group;
                return View(reportView);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id, string accept, string deny)
        {
            var report = await _context.Reports.FindAsync(id);

            if (!string.IsNullOrEmpty(accept))
            {
                if (_context.Reports == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Reports'  is null.");
                }

                if (report.JobId != null)
                {
                    var job = await _context.Jobs.FindAsync(report.JobId);
                    _context.Jobs.Remove(job);
                }
                else if (report.PostId != null)
                {
                    var post = await _context.Posts.FindAsync(report.PostId);
                    _context.Posts.Remove(post);
                }
                else if (report.GroupId != null)
                {
                    var group = await _context.Groups.FindAsync(report.GroupId);
                    _context.Groups.Remove(group);
                }
                if (report != null)
                {
                    _context.Reports.Remove(report);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else if (!string.IsNullOrEmpty(deny))
            {
                if (_context.Reports == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Reports'  is null.");
                }

                if (report != null)
                {
                    _context.Reports.Remove(report);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
