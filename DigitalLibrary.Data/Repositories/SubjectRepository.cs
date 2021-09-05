using System;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Repositories
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public Subject GetSubjectById(Guid subjectId)
        {
            return FindByCondition(subject => subject.Id.Equals(subjectId))
                .FirstOrDefault();
        }
    }
}
