 
using BeFit.Web.Data;
using BeFit.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Web.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationContext _db;
        public TrainingSessionsController(ApplicationContext db) => _db = db;

        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sessions = await _db.TrainingSessions
                                    .Where(s => s.UserId == userId)
                                    .OrderByDescending(s => s.StartAt)
                                    .ToListAsync();
            return View(sessions);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var session = await _db.TrainingSessions
                                   .Include(s => s.PerformedExercises!)
                                   .ThenInclude(pe => pe.ExerciseType)
                                   .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            
            if (session == null) return NotFound();
            return View(session);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Training training)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            if (!ModelState.IsValid)
                return View(training);

            training.UserId = userId;
            _db.TrainingSessions.Add(training);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var session = await _db.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();
            return View(session);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Training training)
        {
            if (id != training.Id) return BadRequest();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var existing = await _db.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (existing == null) return NotFound();
            
            if (!ModelState.IsValid)
                return View(training);

            existing.StartAt = training.StartAt;
            existing.EndAt = training.EndAt;
            
            _db.Update(existing);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var session = await _db.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();
            return View(session);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var session = await _db.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            
            if (session != null)
            {
                _db.TrainingSessions.Remove(session);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
