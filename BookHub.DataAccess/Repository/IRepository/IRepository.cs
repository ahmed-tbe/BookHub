using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookHub.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //to retrieve all instances of category for example
        IEnumerable<T> GetAll(string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        //get a certain category, we use a link operation since it depends on id AND some other condition
        void Add(T obj);
        void Remove(T obj);
        void RemoveRange(IEnumerable<T> items);
    }
}
