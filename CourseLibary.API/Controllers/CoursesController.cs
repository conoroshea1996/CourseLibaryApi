using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibaryService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibaryService, IMapper mapper)
        {
            _courseLibaryService = courseLibaryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> getCoursesForAuthor(Guid authorId )
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }
    
            var courses = await _courseLibaryService.GetCourses(authorId);
            var CourseResult = _mapper.Map<IEnumerable<CourseDto>>(courses);

            return Ok(CourseResult);
        }

        

        [HttpGet("{courseId}",Name = "getCourseForAuthor") ]
        public async Task<ActionResult<CourseDto>> getCourseForAuthor(Guid authorId,Guid courseId)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = await _courseLibaryService.GetCourse(authorId, courseId);

            if(course == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<CourseDto>(course));
        }


        [HttpPost]
        public ActionResult<CourseDto> createCourseForAuthor(Guid authorId,CourseForCreationDto course)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Course>(course);
            _courseLibaryService.AddCourse(authorId, courseEntity);
            _courseLibaryService.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute("getCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId , Guid courseId , CourseForUpdateDto course)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromEntity = _courseLibaryService.GetCourse(authorId, courseId);

            if(courseForAuthorFromEntity == null)
            {
                //maps course for input to entity Course
                var courserToAdd = _mapper.Map<Course>(course);
                courserToAdd.Id = courseId;

                _courseLibaryService.AddCourse(authorId,courserToAdd);
                _courseLibaryService.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courserToAdd);

                return CreatedAtRoute("getCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            //updates key values from one model to the other
            _mapper.Map(course, courseForAuthorFromEntity);

            _courseLibaryService.UpdateCourse(courseForAuthorFromEntity);

            _courseLibaryService.Save();
            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartialUpdateCourseForAuthor(
            Guid authorId, 
            Guid courseId,
            JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromEntity = _courseLibaryService.GetCourse(authorId, courseId);
            
            if(courseForAuthorFromEntity == null)
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = _mapper.Map<Course>(courseDto);
                courseToAdd.Id = courseId;
                _courseLibaryService.AddCourse(authorId, courseToAdd);
                _courseLibaryService.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("getCourseForAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseForAuthorFromEntity);
            patchDocument.ApplyTo(courseToPatch,ModelState);


            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(courseToPatch, courseForAuthorFromEntity);
            _courseLibaryService.UpdateCourse(courseForAuthorFromEntity);
            _courseLibaryService.Save();

            return NoContent();
        }
        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromEntity = _courseLibaryService.GetCourse(authorId, courseId);

            if(courseForAuthorFromEntity == null)
            {
                return NotFound();
            }
            _courseLibaryService.DeleteCourse(courseForAuthorFromEntity);
            _courseLibaryService.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

    }
}
