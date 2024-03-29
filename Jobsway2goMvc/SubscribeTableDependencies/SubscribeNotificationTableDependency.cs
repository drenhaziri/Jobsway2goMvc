﻿using Jobsway2goMvc.Hubs;
using Jobsway2goMvc.Interfaces;
using Jobsway2goMvc.Models;
using TableDependency.SqlClient;

namespace Jobsway2goMvc.SubscribeTableDependencies
{
    public class SubscribeNotificationTableDependency : ISubscribeTableDependency
    {
        SqlTableDependency<Notification> tableDependency;
        NotificationHub _notificationHub;

        public SubscribeNotificationTableDependency(NotificationHub notificationHub)
        {
            _notificationHub = notificationHub;
        }

        public void SubscribeTableDependency(string connectionString)
        {
             tableDependency = new SqlTableDependency<Notification>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();

        }
        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Notification)} SqlTableDependency error: {e.Error.Message}");
        }
        private async void TableDependency_OnChanged (object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Notification> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                var notification = e.Entity;
                if (notification.MessageType == "All")
                {
                    await _notificationHub.SendNotificationToAll(notification.Message);
                }
                else if (notification.MessageType == "Personal")
                {
                    await _notificationHub.SendNotificationToClient(notification.Message, notification.UserName);
                }
            }
        }
    }
}
