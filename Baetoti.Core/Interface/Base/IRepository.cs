using System.Collections.Generic;

namespace Baetoti.Core.Interface.Base
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);

        IEnumerable<T> ListAll();

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

    }
}
