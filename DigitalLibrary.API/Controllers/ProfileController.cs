using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DigitalLibrary.API.Services.FileManager;
using DigitalLibrary.Data;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.ProfileModels;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Profile = DigitalLibrary.Models.Entities.Profile;

namespace DigitalLibrary.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public class ProfileController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FileManager _fileManager;

        public ProfileController(IRepositoryWrapper repository,
            IMapper mapper, UserManager<IdentityUser> userManager, FileManager fileManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(userId);

            var profile = _repository.Profile.GetProfileById(new Guid(userId));

            var profileToReturn = _mapper.Map<ProfileDto>(profile);

            if (profileToReturn == null)
            {
                profileToReturn = new ProfileDto();
            }

            profileToReturn.Email = user.Email;
            profileToReturn.UserName = user.UserName;
            return Ok(profileToReturn);
        }

        [HttpGet]
        [Route("{profileId}", Name = "GetProfile")]
        public async Task<IActionResult> GetProfile(Guid profileId)
        {
            var user = await _userManager.FindByIdAsync(profileId.ToString());

            var profile = _repository.Profile.GetProfileById(profileId);

            var profileToReturn = _mapper.Map<ProfileDto>(profile);
            profileToReturn.Email = user.Email;
            profileToReturn.UserName = user.UserName;
            return Ok(profileToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddProfile(ProfileForCreationDto dto)
        {
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(userId);

            // Проверяем есть ли уже профиль
            var profile = _repository.Profile.GetProfileById(new Guid(userId));
            if (profile != null)
            {
                return BadRequest("ProfileExist");
            }

            // Проверяем существует ли библиотека
            var library = _repository.Library.FindByCondition(x => x.Id.Equals(dto.RegisteredLibraryId))
                .FirstOrDefault();
            if (library == null)
            {
                return BadRequest("Library doesn't exist");
            }

            //Сохранить профиль в таблице профилей
            profile = _mapper.Map<Profile>(dto);
            _repository.Profile.CreateProfileWithLibrary(library.Id, new Guid(userId), profile);

            _repository.Save();

            var profileToReturn = _mapper.Map<ProfileDto>(profile);
            profileToReturn.Email = user.Email;
            profileToReturn.UserName = user.UserName;
            return CreatedAtRoute("GetProfile", new
            {
                profileId = profileToReturn.Id
            }, profileToReturn);

            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateProfile(ProfileForCreationDto profile)
        {
            var profileEntity = _mapper.Map<Profile>(profile);
            var registeredLibrary = profile.RegisteredLibraryId;
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            _repository.Profile.UpdateProfileWithLibrary(profileEntity, registeredLibrary, new Guid(userId));

            _repository.Save();

            var profileToReturn = _repository.Profile.GetProfileById(new Guid(userId));
            return Ok(new
            {
                profile = _mapper.Map<ProfileDto>(profileToReturn)
            });
        }

        [HttpPost]
        [Route("image")]
        public IActionResult UploadImage()
        {
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            try
            {
                var file = Request.Form.Files[0];
                _fileManager.UploadImage(file, userId);
            }
            catch (Exception e)
            {
                return Ok();
            }


            return Ok();
        }


        [HttpGet]
        [Route("image")]
        public IActionResult DownloadImage()
        {
            string userId = (from claim in User.Claims where claim.Type == "sub" select claim.Value).FirstOrDefault();

            try
            {
                return File(_fileManager.GetImageStream(userId), "image/jpeg", "609c9cfe-3d55-4765-ad3d-f63a576310cf.jpeg");
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}
