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
    public class TCSService : ITCSService
    {
        private readonly IMongoCollection<TCS> _TCS;


        public TCSService(IDbClient dbClient)
        {
            _TCS = dbClient.GetTCSCollections();
        }

        public async Task<string> AddAsync(TCSRequest tCSDto)
        {
            var model = GetModel(tCSDto);
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;
            model.IsCompleted = false;
            model.IsDel = false;
            try
            {
                await _TCS.InsertOneAsync(model);
                return model.Id;
            }
            catch (Exception ex) { }
            return string.Empty;
        }

        public async Task<List<TCSResponse>> GetTCsAsync()
        {

            var models = await _TCS.Find(x => !x.IsDel).ToListAsync();
            var response = GetResponseList(models);
            return response;
        }

        private List<TCSResponse> GetResponseList(List<TCS> models)
        {
            var responseList = new List<TCSResponse>();


            foreach (var model in models.OrderByDescending(o => o.UpdatedDate))
            {
                List<string> imgs = new List<string>();
                foreach (var img in model.Images)
                {
                    imgs.Add(string.Format("{0}/{1}", model.Id, img));
                }

                responseList.Add(new TCSResponse
                {
                    Id = model.Id,
                    TCNumber = model.TCNumber,
                    Partprocess = model.Partprocess,
                    Images = imgs,//model.Images,
                    Assymanual = model.Assymanual,
                    Changetype = model.Changetype,
                    Date = model.Date.Date,
                    Effectivelot = model.Effectivelot,
                    Model = model.Model,
                    Newpart = model.Newpart,
                    Oldpart = model.Oldpart,
                    PM = model.PM,
                    RM = model.RM,
                    Variant = model.Variant,
                    Qty = model.Qty,
                    Dispocode = model.DispoCode,
                    Exchangeable = model.Exchangeable,
                    IsCompleted = model.IsCompleted,
                    IsActive = model.IsActive,
                    Completed = model.IsCompleted ? "Completed" : "Incomplete",
                    AbleToExchange = model.Exchangeable ? "Yes" : "No"
                });

            }

            return responseList;
        }
        private TCS GetModel(TCSRequest tCSDto)
        {
            return new TCS
            {
                Model = tCSDto.Model,
                TCNumber = tCSDto.TCNumber,
                Variant = tCSDto.Variant,
                Images = tCSDto.Images,
                Assymanual = tCSDto.Assymanual,
                Date = tCSDto.Date.ToLocalTime(),
                Newpart = tCSDto.Newpart,
                Oldpart = tCSDto.Oldpart,
                Partprocess = tCSDto.Partprocess,
                PM = tCSDto.PM,
                RM = tCSDto.RM,
                CreatedBy = tCSDto.CreatedBy,
                Effectivelot = tCSDto.Effectivelot,
                UpdatedBy = tCSDto.UpdatedBy,
                Changetype = tCSDto.Changetype,
                Qty = tCSDto.Qty,
                DispoCode = tCSDto.DispoCode,
                Exchangeable = tCSDto.Exchangeable,
                IsCompleted = tCSDto.IsCompleted,
                IsActive = tCSDto.IsActive
            };
        }

        public async Task<TCSResponse> GetTCAsync(string id)
        {
            var model = await _TCS.Find(x => x.Id == id && !x.IsDel).FirstOrDefaultAsync();
            var response = this.GetResponseList(new List<TCS> { model });
            return response.FirstOrDefault();
        }

        public async Task<bool> UpdateTCAsync(string id, TCSRequest tCSDto)
        {

            var existModel = await _TCS.Find(x => x.Id == id).FirstOrDefaultAsync();
            var model = GetModel(tCSDto);
            model.CreatedBy = existModel.CreatedBy;
            model.CreatedDate = existModel.CreatedDate;

            if (tCSDto.IsCompleted)
                model.IsActive = false;

            model.IsCompleted = tCSDto.IsCompleted;
            model.UpdatedDate = DateTime.Now;
            model.IsDel = false;
            model.Id = id;
            try
            {
                var result = await _TCS.ReplaceOneAsync(x => x.Id == id, model);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public async Task<bool> SetCompleted(string id)
        {
            try
            {
                var model = await _TCS.Find(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (model != null)
                {
                    model.IsCompleted = true;
                    model.IsActive = false;
                    await _TCS.ReplaceOneAsync(x => x.Id == id, model);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }


        private bool IsDuringPeroid(DateTime impDate, int dayBeforeImp, int dayAfterImp)
        {

            int dayAfterDiff = 0;
            int dayBeforeDiff = 0;
            if (impDate >= DateTime.Now.Date)
            {
                dayBeforeDiff = (int)(impDate - DateTime.Now.Date).TotalDays;
                if (dayBeforeDiff <= dayBeforeImp)
                    return true;
            }
            else
            {
                dayAfterDiff = (int)(DateTime.Now.Date - impDate).TotalDays;
                if (dayAfterDiff <= dayAfterImp)
                    return true;
            }
            return false;
        }

        private bool IsOutOfImplementDate(DateTime impDate, int dayAfterImp)
        {
            int dayAfterDiff = 0;
            if (impDate < DateTime.Now.Date)
            {
                dayAfterDiff = (int)(DateTime.Now.Date - impDate).TotalDays;
                if (dayAfterDiff > dayAfterImp)
                    return true;
            }
            return false;
        }


        public async Task<List<TCSResponse>> GetTCImplementByConfigAsync(int dayBeforeImp, int dayAfterImp)
        {
            try
            {
                var models = await _TCS.Find(x => x.IsActive && !x.IsDel).ToListAsync();
                List<TCS> filterList = new List<TCS>();

                foreach (var model in models)
                    if (IsDuringPeroid(model.Date.Date, dayBeforeImp, dayAfterImp))
                        filterList.Add(model);


                var response = GetResponseList(filterList);
                return response.OrderBy(o => o.Date).ToList();
            }
            catch (Exception ex) { }
            return null;
        }

        public async Task<List<TCSResponse>> GetRemider(int dayBeforeImp, int dayAfterImp)
        {
            var models = await _TCS.Find(x => x.IsActive && !x.IsCompleted).ToListAsync();
            List<TCS> filterList = new List<TCS>();
            foreach (var model in models)
                if (IsOutOfImplementDate(model.Date.Date, dayAfterImp))
                    filterList.Add(model);

            var response = GetResponseList(filterList);
            return response.OrderBy(o => o.Date).ToList();
        }

        public async Task<int> GetCountRemider(int dayBeforeImp, int dayAfterImp)
        {
            int count = 0;
            var models = await _TCS.Find(x => x.IsActive && !x.IsCompleted).ToListAsync();
            List<TCS> filterList = new List<TCS>();
            foreach (var model in models)
                if (IsOutOfImplementDate(model.Date.Date, dayAfterImp))
                    count++;

            return count;
        }


        public async Task<bool> DeleteAsync(string id, string UpdateBy)
        {
            try
            {
                var model = await _TCS.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (model != null)
                {
                    model.IsActive = false;
                    model.UpdatedBy = UpdateBy;
                    model.UpdatedDate = DateTime.Now.ToLocalTime();
                    model.IsDel = true;
                    var result = await _TCS.ReplaceOneAsync(x => x.Id == id, model);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }


    }
}
