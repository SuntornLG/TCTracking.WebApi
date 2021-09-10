
using System.Collections.Generic;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;

namespace TCTracking.Service.Interface
{
    public interface ITrackingAlertService
    {

        Task<bool> SetConfig(TrackingAlertConfigRequest alert);

        Task<TrackingAlertConfigResponse> GetTrackingAlert();

        Task<List<TrackingAlertListResponse>> GetDisplayTrackingAlert();
    }
}
