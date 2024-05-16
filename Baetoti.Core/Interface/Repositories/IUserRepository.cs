using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.User;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IUserRepository : IAsyncRepository<User>
    {
        Task<User> GetByMobileNumberAsync(string mobileNumber);
        Task<User> GetByEmailAsync(string email);
        Task<OnBoardingResponse> GetOnBoardingDataAsync(OnBoardingRequest onBoardingRequest);
        Task<UserResponse> GetAllUsersDataAsync();
        Task<UserProfile> GetUserProfile(long UserID);
        Task<PaginationResponse> GetFilteredUsersDataAsync(UserFilterRequest request);
        Task<BookmarkAndVisitResponse> GetBookmarkAndAccountVisit(long UserID, int UserType);
        Task<List<UserSearchResponse>> SearchUser(UserSearchRequest request);
        Task<List<UserSearchResponse>> SearchFilterUser(UserFliterSearchRequest request);
    }
}
