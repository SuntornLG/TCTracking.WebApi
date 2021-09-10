

namespace TCTracking.Core.Interface
{
   public interface ITCTrackingDatabaseSettings
    {
        string TCS_Collection_Name { get; set; }
        string Users_Collection_Name { get; set; }
        string Notification_Collection_Name { get; set; }
        string Connection_String { get; set; }
        string Database_Name { get; set; }
    }
}
