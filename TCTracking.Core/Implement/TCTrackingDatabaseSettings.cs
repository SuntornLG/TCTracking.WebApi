
using TCTracking.Core.Interface;

namespace TCTracking.Core.Implement
{
    public class TCTrackingDatabaseSettings : ITCTrackingDatabaseSettings
    {
        public string TCS_Collection_Name { get; set; }
        public string Connection_String { get; set; }
        public string Database_Name { get; set; }
        public string Notification_Collection_Name { get; set; }
        public string Users_Collection_Name { get; set; }
    }
}
