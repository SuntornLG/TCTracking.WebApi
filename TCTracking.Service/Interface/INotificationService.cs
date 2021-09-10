
using System.Collections.Generic;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;

namespace TCTracking.Service.Interface
{
   public interface INotificationService
    {
        Task<List<NotificationResponse>> GetAllAsync();
        Task<bool> AddAsync(NotificationRequest request);
        Task<NotificationResponse> GetByIdAsync(string id);

        Task<bool> UpdateAsync(string id, NotificationRequest request);

        Task<bool> DeleteAsync(string id, string UpdateBy);


        Task<List<NotificationResponse>> GetActiveItems();


    }
}
