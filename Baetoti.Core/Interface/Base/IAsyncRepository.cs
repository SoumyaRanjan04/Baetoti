using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Base
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);

        Task<T> GetByIdAsync(long id);

        Task<IReadOnlyList<T>> ListAllAsync();

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task<T> AddAsync(T entity);

        Task<List<T>> AddRangeAsync(List<T> entity);

        Task<List<T>> UpdateRangeAsync(List<T> entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteRangeAsync(List<T> entity);

        Task<int> CountAsync(ISpecification<T> spec);

    }
}
