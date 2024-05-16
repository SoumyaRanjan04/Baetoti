using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IPushNotificationRepository : IAsyncRepository<PushNotification>
    {
        Task<List<PushNotificationResponse>> GetAll();

        Task<PushNotificationResponse> GetByID(long ID);

        Task<PushNotification> GetByNotificationType(int NotificationType);

    }
}
