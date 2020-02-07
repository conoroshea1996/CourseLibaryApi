using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParamenters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]

    //[Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibaryService;
        private readonly IMapper _mapper;

        public AuthorController(ICourseLibraryRepository courseLibaryService, IMapper mapper)
        {
            _courseLibaryService = courseLibaryService;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> getAuthors(
           [FromQuery] AuthorsResourceParameters authorsResourceParameters, int pageIndex = 0, int Results = 2)
        {
            var AuthoursFromEntity = await _courseLibaryService.GetAuthors(authorsResourceParameters);
            AuthoursFromEntity = AuthoursFromEntity.Skip(pageIndex).Take(Results);
            var Authors = _mapper.Map<IEnumerable<AuthorDto>>(AuthoursFromEntity);
            return Ok(Authors);
        }

        [HttpGet("{AuthorId}",Name = "getAuthor")]
        public async Task<ActionResult<AuthorDto>> getAuthor(Guid AuthorId)
        {
            var Author = await _courseLibaryService.GetAuthor(AuthorId);
            var test =  _courseLibaryService.GetAuthor(AuthorId);
            var x = await test;
 
            if (Author == null)
            {
                return NotFound();
            }
            var AuthorResult = _mapper.Map<AuthorDto>(Author);
            return Ok(AuthorResult);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor( AuthorForCreationDto author)
        {
            if(author == null)
            {
                return BadRequest();
            }
            //creating Author entity from AuthorForCreation
            var authorEntity = _mapper.Map<Author>(author);
            _courseLibaryService.AddAuthor(authorEntity);
            await _courseLibaryService.Save();
            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("getAuthor",new { AuthorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpDelete("{authorId}")]
        public async Task<ActionResult> DeleteAuthor(Guid authorId)
        {
            var authorRepo = await _courseLibaryService.GetAuthor(authorId);

            if(authorRepo == null)
            {
                return NotFound();
            }

            _courseLibaryService.DeleteAuthor(authorRepo);
            await _courseLibaryService.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

    }
}
