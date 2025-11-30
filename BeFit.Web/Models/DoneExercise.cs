using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Web.Models
{
    public class DoneExercise
    {
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int ExerciseTypeId { get; set; }

        public string? UserId { get; set; }

        [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000 kg.")]
        [Display(Name = "Weight (kg)", Description = "Weight used for the exercise")]
        [Precision(5, 2)]
        public decimal Weight { get; set; }

        [Range(1, 100, ErrorMessage = "Number of sets must be between 1 and 100.")]
        [Display(Name = "Sets", Description = "How many sets were performed")]
        public int Sets { get; set; }

        [Range(1, 1000, ErrorMessage = "Repetitions must be between 1 and 1000.")]
        [Display(Name = "Repetitions per set", Description = "How many repetitions in one set")]
        public int Repetitions { get; set; }

    
        public Training? TrainingSession { get; set; }
        public ExerciseTyp? ExerciseType { get; set; }
    }
}
