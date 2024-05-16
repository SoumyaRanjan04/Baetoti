using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Notification;
using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface INotificationRepository : IAsyncRepository<Notification>
    {
        Task<List<Notification>> GetFilteredUser(NotificationRequest request, long EmployeeId);

        Task<PaginationResponse> GetAll(GetAllNotificationRequest request, long UserID);

        Task<NotificationResponse> GetByID(long ID);

    }
}
