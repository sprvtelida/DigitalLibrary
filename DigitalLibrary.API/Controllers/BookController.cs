using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.BookModels;
using DigitalLibrary.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using DigitalLibrary.Models.Parameters;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using MimeKit;
using Newtonsoft.Json;

namespace DigitalLibrary.API.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BookController(IRepositoryWrapper repository, IMapper mapper, IWebHostEnvironment env)
        {
            _repository = repository;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        [Route("{bookId}", Name = "GetBook")]
        public ActionResult<BookDto> GetBook(Guid bookId, [FromQuery] string libraryId)
        {
            var bookEntity = _repository.Book.GetBookById(bookId);
            if (bookEntity == null)
            {
                return NotFound();
            }

            var bookToReturn = _mapper.Map<BookDto>(bookEntity);

            if (!libraryId.IsNullOrEmpty())
            {
                var isInStorage = _repository.Storage.IsInStorage(bookId, new Guid(libraryId));
                bookToReturn.IsInStorage = isInStorage;
            }

            return Ok(bookToReturn);
        }

        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetBooks([FromQuery] BookParameters bookParameters)
        {
            var bookEntities = _repository.Book.GetBooks(bookParameters);
            if (bookEntities.IsNullOrEmpty())
            {
                return NotFound();
            };
            var metadata = new
            {
                bookEntities.TotalCount,
                bookEntities.PageSize,
                bookEntities.CurrentPage,
                bookEntities.TotalPages,
                bookEntities.HasNext,
                bookEntities.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var booksToReturn = _mapper.Map<IEnumerable<BookDto>>(bookEntities);

            if (bookParameters.LibraryId != null)
            {
                var guidsOfStoredBooks = _repository.Storage.GetStoredBooksGuids(new Guid(bookParameters.LibraryId));

                foreach (var book in booksToReturn)
                {
                    if (guidsOfStoredBooks.Contains(book.Id))
                    {
                        book.IsInStorage = true;
                    }
                }
            }

            if (bookParameters.OnlyInStorage == true)
            {
                booksToReturn = booksToReturn.Where(book => book.IsInStorage == true);
            }

            return Ok(booksToReturn);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpPost]
        public IActionResult AddBook(BookForCreationDto bookForCreationDto)
        {
            var bookEntity = _mapper.Map<Book>(bookForCreationDto);

            _repository.Book.CreateBook(bookEntity);
            _repository.Save();

            if (bookForCreationDto.GenreId != null)
                _repository.Book.AddGenre(bookEntity.Id, new Guid(bookForCreationDto.GenreId));

            if (bookForCreationDto.SubjectId != null)
                _repository.Book.AddSubject(bookEntity.Id, new Guid(bookForCreationDto.SubjectId));

            if (bookForCreationDto.AuthorId != null)
                _repository.Book.AddAuthor(bookEntity.Id, new Guid(bookForCreationDto.AuthorId));

            if (bookForCreationDto.PublisherId != null)
                _repository.Book.AddPublisher(bookEntity.Id, new Guid(bookForCreationDto.PublisherId));

            // Сохраняем добавленный жанр, сабджект, автора и публишера
            _repository.Save();

            var bookDto = _mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBook", new
            {
                bookId = bookEntity.Id
            }, bookDto);
        }

        [HttpPut]
        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        public IActionResult UpdateBook(BookForUpdateDto bookToUpdate)
        {
            var old = _repository.Book.GetBookById(new Guid(bookToUpdate.Id));
            var updated = _mapper.Map<BookForUpdateDto, Book>(bookToUpdate);

            updated.Genre = _repository.Genre.GetGenreById(new Guid(bookToUpdate.GenreId));
            updated.Author = _repository.Author.GetAuthorById(new Guid(bookToUpdate.AuthorId));
            updated.Subject = _repository.Subject.GetSubjectById(new Guid(bookToUpdate.SubjectId));
            updated.Publisher = _repository.Publisher.FindByCondition(x => x.Id == new Guid(bookToUpdate.PublisherId)).FirstOrDefault();

            _repository.Book.UpdateBook(old, updated);
            _repository.Save();
            return Ok();
        }

        [Route("subjectCollection")]
        [HttpGet]
        public ActionResult<IEnumerable<SubjectDto>> GetSubjects()
        {
            var subjects = _repository.Subject.FindAll().ToList();
            if (subjects.IsNullOrEmpty())
            {
                return NotFound();
            }

            var subjectDtos = _mapper.Map<IEnumerable<SubjectDto>>(subjects);
            return Ok(subjectDtos);
        }

        //[Route("subjectCollection")]
        [HttpGet("subjectCollection/{subjectId}", Name = "GetSubject")]
        public ActionResult<SubjectDto> GetSubject(Guid subjectId)
        {
            var subject = _repository.Subject.FindByCondition(x => x.Id == subjectId).FirstOrDefault();
            if (subject == null)
            {
                return NotFound();
            }

            var subjectDto = _mapper.Map<SubjectDto>(subject);
            return Ok(subjectDto);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [Route("subjectCollection")]
        [HttpPost]
        public ActionResult<SubjectDto> AddSubject([FromQuery] SubjectForCreationDto subjectForCreationDto)
        {
            var subjectEntity = _mapper.Map<Subject>(subjectForCreationDto);
            _repository.Subject.Create(subjectEntity);
            _repository.Save();

            var subjectDto = _mapper.Map<SubjectDto>(subjectEntity);

            return CreatedAtRoute("GetSubject", new
            {
                subjectId = subjectEntity.Id
            }, subjectDto);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpDelete("subjectCollection/{subjectId}")]
        public ActionResult DeleteSubject(Guid subjectId)
        {
            var subjectEntity = _repository.Subject.FindByCondition(x => x.Id == subjectId)
                .FirstOrDefault();

            if (subjectEntity == null)
            {
                return NotFound();
            }

            _repository.Subject.Delete(subjectEntity);
            _repository.Save();

            return NoContent();
        }

        [Route("genreCollection")]
        [HttpGet]
        public ActionResult<IEnumerable<GenreDto>> GetGenres()
        {
            var genres = _repository.Genre.FindAll().ToList();
            if (genres.IsNullOrEmpty())
            {
                return NotFound();
            }

            var genreDtos = _mapper.Map<IEnumerable<GenreDto>>(genres);
            return Ok(genreDtos);
        }

        [HttpGet("genreCollection/{genreId}", Name = "GetGenre")]
        public ActionResult<GenreDto> GetGenre(Guid genreId)
        {
            var genre = _repository.Genre.FindByCondition(x => x.Id == genreId).FirstOrDefault();
            if (genre == null)
            {
                return NotFound();
            }

            var genreDto = _mapper.Map<GenreDto>(genre);
            return Ok(genreDto);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [Route("genreCollection")]
        [HttpPost]
        public ActionResult<GenreDto> AddGenre([FromQuery] GenreForCreationDto genreForCreationDto)
        {
            var genreEntity = _mapper.Map<Genre>(genreForCreationDto);
            _repository.Genre.Create(genreEntity);
            _repository.Save();

            var genreDto = _mapper.Map<GenreDto>(genreEntity);

            return CreatedAtRoute("GetGenre", new
            {
                genreId = genreEntity.Id
            }, genreDto);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpDelete("genreCollection/{genreId}")]
        public ActionResult DeleteGenre(Guid genreId)
        {
            var genreEntity = _repository.Genre.FindByCondition(x => x.Id == genreId)
                .FirstOrDefault();

            if (genreEntity == null)
            {
                return NotFound();
            }

            _repository.Genre.Delete(genreEntity);
            _repository.Save();

            return NoContent();
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [RequestSizeLimit(100_000_000)]
        [HttpPost("files")]
        public IActionResult UploadFiles()
        {
            var bookId = HttpContext.Request.Headers["bookId"][0];

            var book = _repository.Book.GetBookById(new Guid(bookId));

            foreach (var file in HttpContext.Request.Form.Files)
            {
                switch (file.ContentType)
                {
                    case "image/jpg":
                    case "image/jpeg":
                    case "image/png":
                        var dir = Path.Combine(_env.WebRootPath, "images");
                        using (var fileStream =
                            new FileStream(Path.Combine(dir, string.Concat(bookId, ".jpeg")), FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                        }
                        book.Image = bookId;
                        break;
                    case "application/pdf":
                        dir = Path.Combine(_env.WebRootPath, "files");
                        using (var fileStream =
                            new FileStream(Path.Combine(dir, string.Concat(bookId, ".pdf")), FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                        }
                        book.Pdf = bookId;
                        break;
                    case "application/epub":
                    case "application/epub+zip":
                        dir = Path.Combine(_env.WebRootPath, "files");
                        using (var fileStream =
                            new FileStream(Path.Combine(dir, string.Concat(bookId, ".epub")), FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                        }
                        book.Epub = bookId;
                        break;
                    case "application/fb2":
                        dir = Path.Combine(_env.WebRootPath, "files");
                        using (var fileStream =
                            new FileStream(Path.Combine(dir, string.Concat(bookId, ".fb2")), FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                        }
                        book.Fb2 = bookId;
                        break;
                }
            }

            _repository.Book.UpdateBook(book, book);
            _repository.Save();

            return Ok();
        }

        [HttpGet]
        [Route("file")]
        public IActionResult GetFile([FromQuery] string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "files", fileName);
            return PhysicalFile(filePath, MimeTypes.GetMimeType(filePath), Path.GetFileName(filePath));
        }
    }
}
