 
using BeFit.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Web.Data
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        public DbSet<ExerciseTyp> ExerciseTypes { get; set; } = null!;
        public DbSet<Training> TrainingSessions { get; set; } = null!;
        public DbSet<DoneExercise> PerformedExercises { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<DoneExercise>()
                .HasOne(pe => pe.TrainingSession)
                .WithMany(ts => ts.PerformedExercises)
                .HasForeignKey(pe => pe.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DoneExercise>()
                .HasOne(pe => pe.ExerciseType)
                .WithMany()
                .HasForeignKey(pe => pe.ExerciseTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
