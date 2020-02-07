using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.ResourceParamenters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;

        public CourseLibraryRepository(CourseLibraryContext context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course); 
        }         

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }
  
        public async  Task<Course> GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return await _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Course>> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToListAsync();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public void  AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }
      
        public async Task<Author> GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Authors.Include(author => author.Courses)                                    
                                   .FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _context.Authors.ToListAsync<Author>();
        }
         
        public async Task<IEnumerable<Author>> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return await _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if(authorsResourceParameters == null)
            {
                throw new Exception("Error");
            }

            if (string.IsNullOrWhiteSpace( authorsResourceParameters.MainCategory) && string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                return await GetAuthors();
            }

            var collection = _context.Authors as IQueryable<Author>;

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                authorsResourceParameters.MainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == authorsResourceParameters.MainCategory);
            }
            
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                authorsResourceParameters.SearchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(authorsResourceParameters.SearchQuery)
                                        || a.FirstName.Contains(authorsResourceParameters.SearchQuery)
                                        || a.LastName.Contains(authorsResourceParameters.SearchQuery));
            }

            return await collection.ToListAsync();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public async Task<bool> Save()
        {
            return(await _context.SaveChangesAsync() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               // dispose resources when needed
            }
        }
    }
}
