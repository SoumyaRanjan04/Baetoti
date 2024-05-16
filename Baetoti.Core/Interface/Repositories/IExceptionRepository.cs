using Baetoti.Core.Entites;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IExceptionRepository
    {
        Task<AppException> AddAsync(AppException entity);

    }
}
