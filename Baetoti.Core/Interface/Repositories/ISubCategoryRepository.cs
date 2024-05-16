using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Category;
using Baetoti.Shared.Response.SubCategory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ISubCategoryRepository : IAsyncRepository<SubCategory>
    {
        Task<List<SubCategoryResponse>> GetByCategoryAsync(long id);

        Task<List<SubCategoryResponse>> GetAllByStoreID(long StoreID);
    }
}
