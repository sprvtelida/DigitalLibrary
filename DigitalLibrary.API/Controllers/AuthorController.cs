using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.AuthorModels;
using DigitalLibrary.Models.Entities;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    [Authorize(DigitalLibraryConstants.Policies.Moderation)]
    public class AuthorController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public AuthorController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Author>> GetAuthors()
        {
            var authors = _repository.Author.FindAll().ToList();

            if (authors.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors));
        }

        [HttpGet]
        [Route("{authorId}", Name = "GetAuthor")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _repository.Author.GetAuthorById(authorId);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<Author> AddAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);

            _repository.Author.Create(authorEntity);

            _repository.Save();

            var authorDto = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new
            {
                authorId = authorEntity.Id
            }, authorDto);
        }

        [HttpDelete]
        [Route("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorEntity = _repository.Author.GetAuthorById(authorId);

            if (authorEntity == null)
            {
                return NotFound();
            }

            _repository.Author.Delete(authorEntity);
            _repository.Save();

            return NoContent();
        }
    }
}
