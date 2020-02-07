using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseForCreationDto : CourseForManipulationDto
    {
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{

        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "Title and Description can not be the same.",
        //            new[] { "CourseForCreationDto" }
        //            );
        //    }

        //}

        [Required]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
