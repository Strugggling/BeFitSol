using System.ComponentModel.DataAnnotations;

namespace BeFit.Web.Models
{
    public class ExerciseTyp
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Exercise name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Exercise name", Description = "Short name of the exercise type")]
        public string Name { get; set; } = null!;
    }
}
