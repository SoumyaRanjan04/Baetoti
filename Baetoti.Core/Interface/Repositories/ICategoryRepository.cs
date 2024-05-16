using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface ICategoryRepository : IAsyncRepository<Category>
	{
		Task<List<CategoryResponse>> GetAllByStoreID(long StoreID);

	}
}
