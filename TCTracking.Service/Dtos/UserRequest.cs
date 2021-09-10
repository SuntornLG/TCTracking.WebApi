

namespace TCTracking.Service.Dtos
{
    public class UserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string UserRole { get; set; }

        public bool IsActive { get; set; }
        public bool IsSystemAdmin { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }


    }
}
