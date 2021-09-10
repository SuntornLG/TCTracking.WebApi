

using System;

namespace TCTracking.Service.Dtos
{
   public class UsersResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool? IsSystemAdmin { get; set; }
        public bool? IsActive { get; set; }

        public string UserRoleDisplay { get; set; }

        public string UserRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
