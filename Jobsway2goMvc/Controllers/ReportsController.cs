using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Models;
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
    }
}
