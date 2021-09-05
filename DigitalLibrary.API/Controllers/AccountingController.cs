using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.AccountingModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using IdentityServer4;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Date;

namespace DigitalLibrary.API.Controllers
{
    [Route("api/accounting")]
    [ApiController]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
    public class AccountingController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public AccountingController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpGet]
        public IActionResult GetAccountings()
        {
            var accountings = _repository.Accounting.GetAccountingsWithInfo();
            if (accountings.IsNullOrEmpty())
            {
                return NotFound();
            }

            var accountingToReturn = _mapper.Map<IEnumerable<AccountingDto>>(accountings);
            return Ok(accountingToReturn);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpGet("library/{libraryId:guid}")]
        public IActionResult GetAccountingsForLibrary(Guid libraryId)
        {
            var accountings = _repository.Accounting.GetAccountingsFromLibrary(libraryId);
            if (accountings.IsNullOrEmpty())
            {
                return NotFound();
            }

            var accountingsToReturn = _mapper.Map<IEnumerable<AccountingDto>>(accountings);
            return Ok(accountingsToReturn);
        }

        [HttpGet("user")]
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        public IActionResult GetAccountingsForUser()
        {
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            var accountingsForUser = _repository.Accounting.GetAccountingsWithUserId(new Guid(userId));

            var accountingsToReturn = _mapper.Map<IEnumerable<AccountingDto>>(accountingsForUser);
            return Ok(accountingsToReturn);
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost]
        public IActionResult AddAccounting(AccountingForCreationDto dto)
        {
            // check for date
            if (dto.ReturnDate < DateTime.Now.AddDays(4))
            {
                return BadRequest("Return Date must be at least 5 days into future");
            }

            // get a user by subject id
            var userId = User.Identity.GetSubjectId();
            if (userId == null)
            {
                return NotFound("User is not found");
            }

            // If user already have an accounting, return bad request
            if (_repository.Accounting.FindAll().Any(
                    accounting => (!(accounting.Status == Status.Finished || accounting.Status == Status.Declined) && accounting.Profile.Id.Equals(new Guid(userId)))
                )
            )
            {
                return BadRequest("User already taken a book");
            }

            // find a free book
            var storageId = _repository.Storage.GetFreeBookItemId(dto.LibraryId, dto.BookId);
            if (storageId == null)
            {
                return BadRequest("Book is not available");
            }

            // create an accounting
            var AccountingEntity = _mapper.Map<Accounting>(dto);

            AccountingEntity.RequestDate = DateTime.Now;
            AccountingEntity.Status = Status.Requested;

            var profile = _repository.Profile.GetProfileById(new Guid(userId));
            if (profile == null)
            {
                return BadRequest("Profile is not created for this user");
            }

            _repository.Accounting
                .CreateWithAttachments(
                    new Guid(storageId), dto.BookId, dto.LibraryId, profile.Id, AccountingEntity);


            _repository.Save();

            var accountingToReturn = _mapper.Map<AccountingDto>(AccountingEntity);
            return Ok(accountingToReturn);
        }

        [HttpPatch]
        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        public IActionResult UpdateStatus(AccountingForUpdateDto accountingForUpdateDto)
        {
            var accounting =
                _repository.Accounting.FindByCondition(
                    x => x.Id.Equals(accountingForUpdateDto.Id)
                    ).FirstOrDefault();

            if (accounting == null)
                return NotFound();

            if (accountingForUpdateDto.Status == Status.Accepted)
            {
                if (accounting.Status != Status.Requested)
                    return BadRequest("Status is not 'requested'");
                accounting.Status = Status.Accepted;
            }

            if (accountingForUpdateDto.Status == Status.Declined)
            {
                if (accounting.Status != Status.Requested)
                    return BadRequest("Status is not 'requested'");
                accounting.Status = Status.Declined;
            }

            if (accountingForUpdateDto.Status == Status.Issued)
            {
                if (accounting.Status != Status.Accepted)
                    return BadRequest("Status is not 'accepted'");
                accounting.IssueDate = DateTime.Now;
                accounting.Status = Status.Issued;
            }

            if (accountingForUpdateDto.Status == Status.Finished)
            {
                if (accounting.Status != Status.Issued)
                    return BadRequest("Status is not 'issued'");
                accounting.DateReturned = DateTime.Now;
                accounting.Status = Status.Finished;
            }

            _repository.Accounting.Update(accounting);
            _repository.Save();
            return Ok(accounting);
        }
    }
}
