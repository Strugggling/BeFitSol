using BeFit.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Web.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationContext _db;
        public StatisticsController(ApplicationContext db) => _db = db;

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var fourWeeksAgo = DateTime.UtcNow.AddDays(-28);

            var query = _db.PerformedExercises
                           .Where(pe => pe.UserId == userId)
                           .Where(pe => pe.TrainingSession != null && pe.TrainingSession.StartAt >= fourWeeksAgo)
                           .Include(pe => pe.ExerciseType)
                           .Include(pe => pe.TrainingSession);

            var grouped = query
                .AsEnumerable()
                .GroupBy(pe => new { pe.ExerciseTypeId, Name = pe.ExerciseType?.Name ?? "N/D" })
                .Select(g => new
                {
                    ExerciseTypeId = g.Key.ExerciseTypeId,
                    ExerciseName = g.Key.Name,
                    TimesPerformed = g.Count(),
                    TotalRepetitions = g.Sum(pe => pe.Sets * pe.Repetitions),
                    AverageWeight = g.Any() ? (double?)g.Average(pe => (double)pe.Weight) : null,
                    MaxWeight = g.Any() ? (double?)g.Max(pe => (double)pe.Weight) : null
                }).ToList();

            return View(grouped);
        }
    }
}
