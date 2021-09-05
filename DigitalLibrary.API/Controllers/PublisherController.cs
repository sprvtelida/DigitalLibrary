using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.AuthorModels;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.PublisherModels;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.API.Controllers
{
    [ApiController]
    [Route("api/publishers")]
    public class PublisherController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public PublisherController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PublisherDto>> GetPublishers()
        {
            var publishers = _repository.Publisher.FindAll();

            if (publishers.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<PublisherDto>>(publishers));
        }

        [HttpGet]
        [Route("{publisherId}", Name = "GetPublisher")]
        public ActionResult<PublisherDto> GetPublisher(Guid publisherId)
        {
            var publisher = _repository.Publisher.FindByCondition(x => x.Id == publisherId).FirstOrDefault();

            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PublisherDto>(publisher));
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpPost]
        public ActionResult<PublisherDto> AddPublisher(PublisherForCreationDto publisherForCreationDto)
        {
            var publisherEntity = _mapper.Map<Publisher>(publisherForCreationDto);

            _repository.Publisher.Create(publisherEntity);

            _repository.Save();

            var publisherDto = _mapper.Map<PublisherDto>(publisherEntity);

            return CreatedAtRoute("GetPublisher", new
            {
                publisherId = publisherEntity.Id
            }, publisherDto);
        }

        [Authorize(DigitalLibraryConstants.Policies.Moderation)]
        [HttpDelete]
        [Route("{publisherId}")]
        public ActionResult DeletePublisher(Guid publisherId)
        {
            var publisherEntity = _repository.Publisher.FindByCondition(x => x.Id == publisherId).FirstOrDefault();

            if (publisherEntity == null)
            {
                return NotFound();
            }

            _repository.Publisher.Delete(publisherEntity);
            _repository.Save();

            return NoContent();
        }
    }
}
