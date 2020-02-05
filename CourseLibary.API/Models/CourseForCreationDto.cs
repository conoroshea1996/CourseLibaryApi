using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseForCreationDto : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        
        [MaxLength(1500)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if(Title == Description)
            {
                yield return new ValidationResult(
                    "Title and Description can not be the same.",
                    new[] { "CourseForCreationDto" }
                    );
            }

        }
    }
}
