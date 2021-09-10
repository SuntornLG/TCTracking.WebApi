
using System.Collections.Generic;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;

namespace TCTracking.Service.Interface
{
    public interface IDisplayService
    {
        Task<List<DisplayTrackingResponse>> GetDisplayItems();
    }
}
