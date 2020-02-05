using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<IEnumerable<CourseDto>> getCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }
    
            var courses = _courseLibaryService.GetCourses(authorId);
            var CourseResult = _mapper.Map<IEnumerable<CourseDto>>(courses);

            return Ok(CourseResult);
        }

        [HttpGet("{courseId}",Name = "getCourseForAuthor") ]
        public ActionResult<CourseDto> getCourseForAuthor(Guid authorId,Guid courseId)
        {
            if (!_courseLibaryService.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibaryService.GetCourse(authorId, courseId);

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

    }
}
