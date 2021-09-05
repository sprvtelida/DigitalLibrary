using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data.Repositories
{
    public class ProfileRepository : Repository<Profile>, IProfileRepository
    {
        public ProfileRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public IEnumerable<Profile> GetProfilesWithLibrary()
        {
            return FindAll().Include(lib => lib.RegisteredLibrary).ToList();
        }

        public Profile GetProfileById(Guid profileId)
        {
            return FindByCondition(x => x.Id.Equals(profileId))
                .Include(x => x.RegisteredLibrary).FirstOrDefault();
        }

        public void CreateProfileWithLibrary(Guid libraryId, Guid userId, Profile profile)
        {
            var library = AppDbContext.Libraries.Find(libraryId);
            profile.RegisteredLibrary = library;
            profile.Id = userId;
            Create(profile);
        }

        public void UpdateProfileWithLibrary(Profile profile, Guid libraryId, Guid userId)
        {
            var library = AppDbContext.Libraries.Find(libraryId);
            var profileFromDb = AppDbContext.Profiles.Find(userId);

            profileFromDb.Address = profile.Address;
            profileFromDb.City = profile.City;
            profileFromDb.FirstName = profile.FirstName;
            profileFromDb.LastName = profile.LastName;
            profileFromDb.PhoneNumber = profile.PhoneNumber;
            profileFromDb.RegisteredLibrary = library;
            profileFromDb.IIN = profile.IIN;
        }
    }
}
