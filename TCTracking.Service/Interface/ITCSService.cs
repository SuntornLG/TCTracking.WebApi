
using System.Collections.Generic;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;

namespace TCTracking.Service.Interface
{
    public interface ITCSService
    {

        Task<List<TCSResponse>> GetTCsAsync();

        Task<List<TCSResponse>> GetRemider(int dayBeforeImp, int dayAfterImp);

        Task<int> GetCountRemider(int dayBeforeImp, int dayAfterImp);

        Task<string> AddAsync(TCSRequest tCSDto);
        Task<TCSResponse> GetTCAsync(string id);

        Task<bool> UpdateTCAsync(string id, TCSRequest tCSDto);

        Task<bool> SetCompleted(string id);

        Task<bool> DeleteAsync(string id, string updateBy);

        Task<List<TCSResponse>> GetTCImplementByConfigAsync(int dayBeforeImp, int dayAfterImp);

    }
}
