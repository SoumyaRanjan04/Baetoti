using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ITagsRepository: IAsyncRepository<Tags>
    {
        Task<List<Shared.Response.TagResponse.TagResponse>> GetAllTagsAsync();

        Task<List<Shared.Response.TagResponse.TagResponse>> GetAllStoreTags();

        Task<Shared.Response.TagResponse.TagResponse> GetTagByIDAsync(int ID);

        Task<List<Shared.Response.TagResponse.TagResponse>> GetTagBySubCategoryIDAsync(int SubCategoryID);

    }
}
