using CourseLibrary.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.ValidationAttributes {
    public class CourseTitleMustBeDifferentToDescriptionAttribute : ValidationAttribute {
        public CourseTitleMustBeDifferentToDescriptionAttribute() {
        }

        protected override ValidationResult? IsValid(object? value,
            ValidationContext validationContext) {
            if (validationContext.ObjectInstance is not
                CourseForManipulationDto course) {
                throw new Exception($"Attribute " +
                    $"{nameof(CourseTitleMustBeDifferentToDescriptionAttribute)} " +
                    $"must be applied to a " +
                    $"{nameof(CourseForManipulationDto)} or derived type.");
            }

            if (course.Title == course.Description) {
                return new ValidationResult(
                "The provided description should be different from the title.",
                    new[] { nameof(CourseForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
