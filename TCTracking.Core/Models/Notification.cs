using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TCTracking.Core.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int NotificatonBeforeDateDay { get; set; }
        public int NotificationAfterDateDay { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDel { get; set; }
    }
}
