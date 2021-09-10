using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;

namespace TCTracking.Service.Implement
{
    public class TrackingAlertService : ITrackingAlertService
    {
        public Task<List<TrackingAlertListResponse>> GetDisplayTrackingAlert()
        {
            throw new NotImplementedException();
        }

        public Task<TrackingAlertConfigResponse> GetTrackingAlert()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetConfig(TrackingAlertConfigRequest alert)
        {
            throw new NotImplementedException();
        }
    }
}
