

namespace TCTracking.Service.Dtos
{
    public class DisplayTrackingResponse : TrackingBaseResponse
    {
        public string ImplementDate { get; set; }
        public string DisplayExchangeAble { get; set; }
        public bool IsLastMinute { get; set; }
    }
}
