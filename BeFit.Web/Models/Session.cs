 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Web.Models
{
    public class Training
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Start date and time", Description = "Start time")]
        public DateTime StartAt { get; set; }

        [Required]
        [Display(Name = "End date and time", Description = "End time")]
        [CustomValidation(typeof(Training), nameof(ValidateEndAfterStart))]
        public DateTime EndAt { get; set; }

        
        public string? UserId { get; set; }

        
        public ICollection<DoneExercise>? PerformedExercises { get; set; }

        public static ValidationResult? ValidateEndAfterStart(DateTime endAt, ValidationContext ctx)
        {
            var instance = (Training?)ctx.ObjectInstance;
            if (instance == null) return ValidationResult.Success;

            if (endAt <= instance.StartAt)
                return new ValidationResult("End time must be larger than start time.");
            return ValidationResult.Success;
        }
    }
}
