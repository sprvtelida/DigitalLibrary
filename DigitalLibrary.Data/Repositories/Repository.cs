using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DigitalLibrary.Data.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext AppDbContext { get; set; }

        public Repository(AppDbContext appDbContext)
        {
            this.AppDbContext = appDbContext;
        }

        public IQueryable<T> FindAll()
        {
            return this.AppDbContext.Set<T>()
                .AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.AppDbContext.Set<T>()
                .Where(expression)
                .AsNoTracking();
        }

        public void Create(T entity)
        {
            this.AppDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.AppDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.AppDbContext.Set<T>().Remove(entity);
        }
    }
}
