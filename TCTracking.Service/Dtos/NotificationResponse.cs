
using System;

namespace TCTracking.Service.Dtos
{
    public class NotificationResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int NotificatonBeforeDateDay { get; set; }
        public int NotificationAfterDateDay { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string UpdateDateStr { get; set; }
    }

    public class NotificationCount
    {
        public int Count { get; set; }
    }
}
