
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCTracking.Core.Interface;
using TCTracking.Core.Models;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;

namespace TCTracking.Service.Implement
{
    public class NotificationService : INotificationService
    {

        private readonly IMongoCollection<Notification> _notificationRepository;
        public NotificationService(IDbClient dbClient)
        {
            _notificationRepository = dbClient.GetNotificationCollections();
        }

        public async Task<bool> AddAsync(NotificationRequest request)
        {

            var model = GetModel(request);
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;

            try
            {
                await _notificationRepository.InsertOneAsync(model);
                if (request.IsActive)
                    await SetAllToInactive(model.Id);

                return true;
            }
            catch (Exception ex) { }
            return false;

        }

        private async Task<bool> SetAllToInactive(string id)
        {

            var models = await _notificationRepository
                .Find(x => x.Id != id && !x.IsDel).ToListAsync();

            try
            {
                var currentModel = await _notificationRepository.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (currentModel != null)
                {
                    currentModel.IsActive = true;
                    await _notificationRepository.ReplaceOneAsync(x => x.Id == currentModel.Id, currentModel);
                }
                foreach (var model in models)
                {
                    model.IsActive = false;
                    var result = await _notificationRepository.ReplaceOneAsync(x => x.Id == model.Id, model);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }



        private Notification GetModel(NotificationRequest request)
        {
            var model = new Notification
            {
                Name = request.Name,
                NotificationAfterDateDay = request.NotificationAfterDateDay,
                NotificatonBeforeDateDay = request.NotificatonBeforeDateDay,
                Description = request.Description,
                IsActive = request.IsActive
            };
            return model;
        }

        private List<NotificationResponse> GetResponseList(List<Notification> models)
        {
            List<NotificationResponse> modelList = new List<NotificationResponse>();

            foreach (var model in models.OrderByDescending(o => o.UpdatedDate))
            {
                modelList.Add(new NotificationResponse
                {
                    Id = model.Id,
                    Description = model.Description,
                    Name = model.Name,
                    NotificationAfterDateDay = model.NotificationAfterDateDay,
                    NotificatonBeforeDateDay = model.NotificatonBeforeDateDay,
                    IsActive = model.IsActive,
                    UpdatedDate = model.UpdatedDate.ToLocalTime(),
                    UpdatedBy = model.UpdatedBy,
                    UpdateDateStr = model.UpdatedDate.ToLocalTime().ToString("dd MMM yy HH:mm:ss")
                });
            }

            return modelList;
        }

        public async Task<List<NotificationResponse>> GetAllAsync()
        {
            var models = await _notificationRepository.Find(x => true && !x.IsDel).ToListAsync();
            var response = GetResponseList(models);
            return response;
        }

        public async Task<NotificationResponse> GetByIdAsync(string id)
        {
            var model = await _notificationRepository.Find(x => x.Id == id && !x.IsDel).FirstOrDefaultAsync();
            var response = this.GetResponseList(new List<Notification> { model });
            return response.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(string id, NotificationRequest request)
        {
            var model = GetModel(request);
            model.UpdatedDate = DateTime.Now;
            model.Id = id;
            try
            {
                var result = await _notificationRepository.ReplaceOneAsync(x => x.Id == id, model);
                if (model.IsActive)
                    await SetAllToInactive(model.Id);

                return true;
            }
            catch (Exception) { }
            return false;

        }

        public async Task<List<NotificationResponse>> GetActiveItems()
        {
            var models = await _notificationRepository.Find(x => x.IsActive && !x.IsDel).ToListAsync();
            var response = GetResponseList(models);
            return response;
        }

        public async Task<bool> DeleteAsync(string id, string UpdateBy)
        {
            try
            {
                var model = await _notificationRepository.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (model != null)
                {
                    model.IsActive = false;
                    model.UpdatedBy = UpdateBy;
                    model.UpdatedDate = DateTime.Now.ToLocalTime();
                    model.IsDel = true;
                    var result = await _notificationRepository.ReplaceOneAsync(x => x.Id == id, model);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }
    }
}
