using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
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
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibaryService;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository courseLibaryService, IMapper mapper)
        {
            _courseLibaryService = courseLibaryService;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name ="GetAuthorCollection")]
        public async Task<IActionResult> GetAuthorCollection(
            [FromRoute] 
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids
            )
        { 
            if(ids == null)
            {
                return BadRequest();
            }
            var authorEntites = await _courseLibaryService.GetAuthors(ids);

            if(ids.Count() != authorEntites.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntites);

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> createAuthorCollection(
            IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntityCollection = _mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach(var author in authorEntityCollection)
            {
                _courseLibaryService.AddAuthor(author);
            }

            _courseLibaryService.Save();
            
            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntityCollection);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorCollectionToReturn);
        }
    }
}
