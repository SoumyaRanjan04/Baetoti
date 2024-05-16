using Baetoti.Core.Interface.Base;
using Baetoti.Core.Specifications.Base;
using Baetoti.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories.Base
{
	public class EFRepository<T> : IAsyncRepository<T> where T : class
	{

		private readonly BaetotiDbContext _dbContext;

		public EFRepository(BaetotiDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public virtual async Task<T> GetByIdAsync(Guid id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
		}

		public async Task<IReadOnlyList<T>> ListAllAsync()
		{
			return await _dbContext.Set<T>().ToListAsync();
		}

		public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
		{
			return await ApplySpecification(spec).ToListAsync();
		}

		public async Task<int> CountAsync(ISpecification<T> spec)
		{
			return await ApplySpecification(spec).CountAsync();
		}

		public async Task<T> AddAsync(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<List<T>> AddRangeAsync(List<T> entity)
		{
			await _dbContext.Set<T>().AddRangeAsync(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<List<T>> UpdateRangeAsync(List<T> entity)
		{
			_dbContext.Set<T>().UpdateRange(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task UpdateAsync(T entity)
		{
			_dbContext.Entry(entity).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(T entity)
		{
			_dbContext.Entry(entity).State = EntityState.Deleted;
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteRangeAsync(List<T> entity)
		{
			_dbContext.Set<T>().RemoveRange(entity);
			await _dbContext.SaveChangesAsync();
		}

		private IQueryable<T> ApplySpecification(ISpecification<T> spec)
		{
			return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
		}

		public IEnumerable<T> ListAll()
		{
			return _dbContext.Set<T>().ToList();
		}

		public async Task<T> GetByIdAsync(long id)
		{
			var entity = await _dbContext.Set<T>().FindAsync(id);
			_dbContext.Entry(entity).State = EntityState.Detached;
			return entity;
		}

	}
}
