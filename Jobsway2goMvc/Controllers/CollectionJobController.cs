using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;

namespace Jobsway2goMvc.Controllers
{
    public class CollectionJobController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CollectionJobController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Jobs");
        }

        public async Task<IActionResult> SaveToCollection(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var Jobs = await _context.Jobs.FindAsync(id);

            if (Jobs == null)
            {
                return NotFound();
            }

            var collections = _context.Collections.Where(a => a.User.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            ViewBag.Collections = new SelectList(collections, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveToCollection(int id, Collection collection)
        {
            var collections = _context.Collections.FirstOrDefault(g => g.Id == collection.Id);
            var jobs = _context.Jobs.FirstOrDefault(g => g.Id == id);

            if (collections != null && jobs != null)
            {
                if (!collections.Jobs.Contains(jobs))
                {
                    collections.Jobs.Add(jobs);
                    _context.Collections.Update(collections);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Collections");
                }
            }
            return RedirectToAction("Index", "Jobs");
        }
    }
}
