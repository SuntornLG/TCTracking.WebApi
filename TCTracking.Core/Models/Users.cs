using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TCTracking.Core.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public bool? IsSystemAdmin { get; set; }

        public string UserRole { get; set; }

        public bool? IsActive { get; set; }
        public string Password { get; set; }

        public bool IsDel { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
