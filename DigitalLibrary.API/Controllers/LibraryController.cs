using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.AccountingModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.LibraryModels;
using DigitalLibrary.Models.StorageModels;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DigitalLibrary.API.Controllers
{
    [Route("api/library")]
    [ApiController]
    //[Authorize(Policy = DigitalLibraryConstants.Policies.Moderation)]
    public class LibraryController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;


        public LibraryController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetLibraries()
        {
            var libraries = _repository.Library.FindAll().ToList();

            if (libraries.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<LibraryDto>>(libraries));
        }

        [HttpGet]
        [Route("{libraryId:guid}", Name="GetLibrary")]
        public IActionResult GetLibrary(Guid libraryId)
        {
            var library = _repository.Library.FindByCondition(lib => lib.Id.Equals(libraryId)).FirstOrDefault();
            if (library == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<LibraryDto>(library));
        }

        [Authorize(Policy = DigitalLibraryConstants.Policies.Administration)]
        [HttpPost]
        public IActionResult AddLibrary(LibraryForCreationDto dto)
        {
            var library = _mapper.Map<Library>(dto);

            _repository.Library.Create(library);
            _repository.Save();

            var libraryToReturn = _mapper.Map<LibraryDto>(library);
            return CreatedAtRoute("GetLibrary", new
            {
                libraryId = libraryToReturn.Id
            }, libraryToReturn);
        }

        [Authorize(Policy = DigitalLibraryConstants.Policies.Administration)]
        [HttpDelete]
        [Route("{libraryId:guid}")]
        public IActionResult DeleteLibrary(Guid libraryId)
        {
            var library = _repository.Library.FindByCondition(lib => lib.Id.Equals(libraryId)).FirstOrDefault();
            if (library == null)
            {
                return NotFound();
            }
            _repository.Library.Delete(library);
            _repository.Save();
            return NoContent();
        }

        [Authorize(Policy = DigitalLibraryConstants.Policies.Moderation)]
        [HttpGet]
        [Route("{libraryId:guid}/storage")]
        public IActionResult GetStoredItemsFromLibrary(Guid libraryId)
        {
            var storedItems = _repository.Storage.GetStoredBooks(libraryId);
            if (storedItems.IsNullOrEmpty())
            {
                return NotFound();
            }

            var storedItemsDtos = _mapper.Map<IEnumerable<StoredItemDto>>(storedItems);

            var accountings = _repository.Accounting.GetAccountingsFromLibrary(libraryId);

            foreach (var dto in storedItemsDtos)
            {
                foreach (var accounting in accountings)
                {
                    if (accounting.StoredItem.Id.Equals(dto.Id))
                        dto.Accounting = _mapper.Map<AccountingDto>(accounting);
                }
            }

            return Ok(storedItemsDtos);
        }

        [HttpGet]
        [Route("{libraryId:guid}/storage/{bookId:guid}")]
        public ActionResult<int> GetQuantityOfStoredItemsFromLibrary(Guid libraryId, Guid bookId)
        {
            var books = _repository.Storage.GetStoredBooks(libraryId, bookId);
            if (books.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(books.Count());
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpPost]
        [Route("{libraryId:guid}/storage/{bookId:guid}")]
        public IActionResult AddItemToLibrary(Guid libraryId, Guid bookId, [FromQuery] int quantity = 1)
        {
            var library = _repository.Library.FindByCondition(lib => lib.Id.Equals(libraryId)).FirstOrDefault();
            if (library == null)
            {
                return BadRequest("Library doesn't exist");
            }

            var book = _repository.Book.FindByCondition(book => book.Id.Equals(bookId)).FirstOrDefault();
            if (book == null)
            {
                return BadRequest("Book doesn't exist");
            }

            _repository.Storage.CreateByLibraryIdAndBookId(libraryId, bookId, quantity);
            _repository.Save();
            return Ok();
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpDelete]
        [Route("{libraryId:guid}/storage/{itemId:guid}")]
        public IActionResult DeleteItemFromLibrary(Guid libraryId, Guid itemId)
        {
            var item = _repository.Storage.GetItemByLibraryAndBookId(libraryId, itemId);
            if (item == null)
            {
                return NotFound();
            }
            _repository.Storage.Delete(item);
            _repository.Save();
            return Ok();
        }
    }
}
