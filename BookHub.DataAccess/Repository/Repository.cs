using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookHub.DataAccess.Data;
using BookHub.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookHub.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            this._db = db;
            this.dbSet = _db.Set<T>();
            //DbSet.Categories = dbSet
            _db.Products.Include(u => u.Category);
        }  

        public void Add(T obj)
        {
            dbSet.Add(obj);
        }

        public T Get(Expression<Func<T,bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        //if they add fields like Category, Covertype then they'll be added to the include statement
        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T obj)
        {
            dbSet.Remove(obj);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            dbSet.RemoveRange(items);
        }
    }
}
