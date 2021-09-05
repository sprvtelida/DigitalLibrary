using System;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Subject GetSubjectById(Guid subjectId);
    }
}
