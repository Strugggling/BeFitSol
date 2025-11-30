 
using BeFit.Web.Data;
using BeFit.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Web.Controllers
{
    [Authorize]
    public class PerformedExercisesController : Controller
    {
        private readonly ApplicationContext _db;
        public PerformedExercisesController(ApplicationContext db) => _db = db;

        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var list = await _db.PerformedExercises
                                .Include(pe => pe.ExerciseType)
                                .Include(pe => pe.TrainingSession)
                                .Where(pe => pe.UserId == userId)
                                .OrderByDescending(pe => pe.Id)
                                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewBag.Sessions = await _db.TrainingSessions.Where(s => s.UserId == userId).ToListAsync();
            ViewBag.ExerciseTypes = await _db.ExerciseTypes.OrderBy(e => e.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoneExercise model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            
            var session = await _db.TrainingSessions.FindAsync(model.SessionId);
            if (session == null || session.UserId != userId)
            {
                ModelState.AddModelError(nameof(model.SessionId), "The selected training session doesnt belong to this user");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sessions = await _db.TrainingSessions.Where(s => s.UserId == userId).ToListAsync();
                ViewBag.ExerciseTypes = await _db.ExerciseTypes.OrderBy(e => e.Name).ToListAsync();
                return View(model);
            }

            model.UserId = userId;
            _db.PerformedExercises.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsOwnerAsync(int id)
        {
            var pe = await _db.PerformedExercises.FindAsync(id);
            return pe != null && pe.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await IsOwnerAsync(id)) return Forbid();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var model = await _db.PerformedExercises.FindAsync(id);
            if (model == null) return NotFound();

            ViewBag.Sessions = await _db.TrainingSessions.Where(s => s.UserId == userId).ToListAsync();
            ViewBag.ExerciseTypes = await _db.ExerciseTypes.OrderBy(e => e.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoneExercise model)
        {
            if (id != model.Id) return BadRequest();
            if (!await IsOwnerAsync(id)) return Forbid();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var session = await _db.TrainingSessions.FindAsync(model.SessionId);
            if (session == null || session.UserId != userId)
                ModelState.AddModelError(nameof(model.SessionId), "The selected training session doesnt belong to this user.");

            if (!ModelState.IsValid)
            {
                ViewBag.Sessions = await _db.TrainingSessions.Where(s => s.UserId == userId).ToListAsync();
                ViewBag.ExerciseTypes = await _db.ExerciseTypes.OrderBy(e => e.Name).ToListAsync();
                return View(model);
            }

            var existing = await _db.PerformedExercises.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ExerciseTypeId = model.ExerciseTypeId;
            existing.SessionId = model.SessionId;
            existing.Weight = model.Weight;
            existing.Sets = model.Sets;
            existing.Repetitions = model.Repetitions;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsOwnerAsync(id)) return Forbid();
            var model = await _db.PerformedExercises
                                 .Include(pe => pe.ExerciseType)
                                 .Include(pe => pe.TrainingSession)
                                 .FirstOrDefaultAsync(pe => pe.Id == id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsOwnerAsync(id)) return Forbid();
            var model = await _db.PerformedExercises.FindAsync(id);
            if (model != null)
            {
                _db.PerformedExercises.Remove(model);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
