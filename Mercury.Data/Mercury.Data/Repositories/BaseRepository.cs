using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Data
{
    public abstract class BaseRepository<T> where T : class
    {
        protected IObjectScope scope;

        public BaseRepository(IObjectScope objectScope)
        {
            this.scope = objectScope;
        }

        public virtual void Add(T item)
        {
            scope.Add(item);
        }

        public virtual void Delete(T item)
        {
            scope.Remove(item);
        }

        public virtual IQueryable<T> GetAll()
        {
            return scope.Extent<T>();
        }

        public abstract T GetByID(int id);

        public virtual void Save()
        {
            scope.Transaction.Commit();
        }
    }
}
