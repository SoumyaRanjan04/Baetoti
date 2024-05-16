using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IOTPRepository : IAsyncRepository<OTP>
    {
        Task<OTP> GetByUserIdAsync(long id);
    }
}
