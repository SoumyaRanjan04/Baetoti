using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IUserLoginHistoryRepository : IAsyncRepository<UserLoginHistory>
    {
        Task<UserLoginHistory> GetByUserID(long UserID);

    }
}
