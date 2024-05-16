using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IDriverOrderRepository : IAsyncRepository<DriverOrder>
    {
        Task<DriverOrder> GetByOrderID(long OrderID);

        Task<Driver> GetDriverByOrderID(long OrderID);

    }
}
