using BeFit.Web.Data;
using BeFit.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Web.Controllers
{
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationContext _db;
        public ExerciseTypesController(ApplicationContext db) => _db = db;

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _db.ExerciseTypes.OrderBy(e => e.Name).ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var et = await _db.ExerciseTypes.FindAsync(id);
            if (et == null) return NotFound();
            return View(et);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(ExerciseTyp exerciseType)
        {
            if (!ModelState.IsValid) return View(exerciseType);
            _db.ExerciseTypes.Add(exerciseType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var et = await _db.ExerciseTypes.FindAsync(id);
            if (et == null) return NotFound();
            return View(et);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, ExerciseTyp exerciseType)
        {
            if (id != exerciseType.Id) return BadRequest();
            if (!ModelState.IsValid) return View(exerciseType);

            _db.Update(exerciseType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var et = await _db.ExerciseTypes.FindAsync(id);
            if (et == null) return NotFound();
            return View(et);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var et = await _db.ExerciseTypes.FindAsync(id);
            if (et != null)
            {
                _db.ExerciseTypes.Remove(et);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
