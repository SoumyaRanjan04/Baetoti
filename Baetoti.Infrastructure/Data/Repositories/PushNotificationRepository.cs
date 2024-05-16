using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class PushNotificationRepository : EFRepository<PushNotification>, IPushNotificationRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public PushNotificationRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PushNotificationResponse>> GetAll()
        {
            return await (from pn in _dbContext.PushNotifications
                          join nt in _dbContext.NotificationTypes on pn.NotificationTypeID equals nt.ID
                          select new PushNotificationResponse
                          {
                              ID = pn.ID,
                              NotificationTypeID = pn.NotificationTypeID,
                              NotificationType = nt.Type,
                              Title = pn.Title,
                              TextArabic = pn.TextArabic,
                              Text = pn.Text,
                              TitleArabic = pn.TitleArabic
                          }).ToListAsync();
        }

        public async Task<PushNotificationResponse> GetByID(long ID)
        {
            return await (from pn in _dbContext.PushNotifications
                          join nt in _dbContext.NotificationTypes on pn.NotificationTypeID equals nt.ID
                          where pn.ID == ID
                          select new PushNotificationResponse
                          {
                              ID = pn.ID,
                              NotificationTypeID = pn.NotificationTypeID,
                              NotificationType = nt.Type,
                              Title = pn.Title,
                              TextArabic = pn.TextArabic,
                              Text = pn.Text,
                              TitleArabic = pn.TitleArabic
                          }).FirstOrDefaultAsync();
        }

        public async Task<PushNotification> GetByNotificationType(int NotificationType)
        {
            return await _dbContext.PushNotifications.Where(n => n.NotificationTypeID == NotificationType).FirstOrDefaultAsync();
        }

    }
}
