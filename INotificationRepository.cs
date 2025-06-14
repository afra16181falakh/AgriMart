using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetUserNotifications(Guid userId);
        Task<bool> MarkAsRead(int notificationId);
    }
}