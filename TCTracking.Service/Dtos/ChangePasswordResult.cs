
namespace TCTracking.Service.Dtos
{
   public class ChangePasswordResult
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class ErrorCollection
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }
    }
}
