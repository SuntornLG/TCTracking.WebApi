

namespace TCTracking.Service.Dtos
{
    public class NotificationRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int NotificatonBeforeDateDay { get; set; }
        public int NotificationAfterDateDay { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
