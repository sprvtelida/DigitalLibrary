using System;
using System.Collections;
using System.Collections.Generic;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IProfileRepository : IRepository<Profile>
    {
        Profile GetProfileById(Guid profileId);
        void CreateProfileWithLibrary(Guid libraryId, Guid userId, Profile profile);
        public void UpdateProfileWithLibrary(Profile profile, Guid libraryId, Guid UserId);
        IEnumerable<Profile> GetProfilesWithLibrary();
    }
}
