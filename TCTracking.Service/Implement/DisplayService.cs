using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;

namespace TCTracking.Service.Implement
{
    public class DisplayService : IDisplayService
    {
        private readonly ITCSService _tcsService;
        private readonly INotificationService _notificationService;

        public DisplayService(ITCSService tcsService, INotificationService notificationService)
        {
            _tcsService = tcsService;
            _notificationService = notificationService;
        }

        public async Task<List<DisplayTrackingResponse>> GetDisplayItems()
        {

            int dayBImplement = Constants.Constain.DAY_BEFORE_IMPLEMENT;
            int dayAImplement = Constants.Constain.DAY_AFTER_IMPLEMENT;
            var alertSettings = await _notificationService.GetActiveItems();
            if (alertSettings != null && alertSettings.Any())
            {
                dayBImplement = alertSettings.FirstOrDefault().NotificatonBeforeDateDay;
                dayAImplement = alertSettings.FirstOrDefault().NotificationAfterDateDay;
            }

            var displayItems = await _tcsService.GetTCImplementByConfigAsync(dayBImplement, dayAImplement);
            List<DisplayTrackingResponse> response = new List<DisplayTrackingResponse>();
            if (displayItems != null && displayItems.Any())
            {
                foreach (var item in displayItems)
                {
                    if (item.Images == null)
                    {
                        List<string> imgs = new List<string>();
                        imgs.Add("no_image_available.jpg");
                        imgs.Add("no_image_available.jpg");
                        item.Images = imgs;
                    }
                    else
                    {
                        if (item.Images.Count == 2)
                        {
                            if (string.IsNullOrEmpty(item.Images[0]))
                                item.Images[0] = "no_image_available.jpg";

                            if (string.IsNullOrEmpty(item.Images[1]))
                                item.Images[1] = "no_image_available.jpg";
                        }
                        else if (item.Images.Count == 1)
                        {
                            if (string.IsNullOrEmpty(item.Images[0]))
                                item.Images[0] = "no_image_available.jpg";

                            item.Images.Add("no_image_available.jpg");
                        }
                        else if (item.Images.Count == 0)
                        {
                            List<string> imgs = new List<string>();
                            imgs.Add("no_image_available.jpg");
                            imgs.Add("no_image_available.jpg");
                            item.Images = imgs;
                        }

                        if (item.Images.Count == 2)
                        {
                            List<string> imgsItem = new List<string>();

                            var oldImage = item.Images.Where(x => x.Trim().ToLower().Contains("old")).FirstOrDefault();
                            var newImage = item.Images.Where(x => x.Trim().ToLower().Contains("new")).FirstOrDefault();

                            if (!string.IsNullOrEmpty(oldImage))
                                imgsItem.Add(oldImage);
                            else
                                imgsItem.Add("no_image_available.jpg");

                            if (!string.IsNullOrEmpty(newImage))
                                imgsItem.Add(newImage);
                            else
                                imgsItem.Add("no_image_available.jpg");


                            item.Images = imgsItem;

                        }

                    }

                    string display = "No";
                    if (item.Exchangeable != null)
                        display = item.Exchangeable.Value ? "Yes" : "No";

                    var date = item.Date.ToLocalTime();

                    var isLastMinute = false;
                    if (date.Date == DateTime.Now.Date || date.Date == DateTime.Now.Date.AddDays(-1).Date)
                        isLastMinute = true;


                    response.Add(new DisplayTrackingResponse
                    {
                        TCNumber = item.TCNumber,
                        Model = string.IsNullOrEmpty(item.Model) ? "NA" : item.Model,
                        Changetype = string.IsNullOrEmpty(item.Changetype) ? "NA" : item.Changetype,
                        Assymanual = string.IsNullOrEmpty(item.Assymanual) ? "NA" : item.Assymanual,
                        Date = item.Date.ToLocalTime(),
                        ImplementDate = item.Date.ToString("dd MMMM yy"),
                        Newpart = string.IsNullOrEmpty(item.Newpart) ? "NA" : item.Newpart,
                        Oldpart = string.IsNullOrEmpty(item.Oldpart) ? "NA" : item.Oldpart,
                        Effectivelot = string.IsNullOrEmpty(item.Effectivelot) ? "NA" : item.Effectivelot,
                        Partprocess = string.IsNullOrEmpty(item.Partprocess) ? "NA" : item.Partprocess,
                        Variant = string.IsNullOrEmpty(item.Variant) ? "NA" : item.Variant,
                        PM = string.IsNullOrEmpty(item.PM) ? "NA" : item.PM,
                        RM = string.IsNullOrEmpty(item.RM) ? "NA" : item.RM,
                        Images = item.Images,
                        Qty = item.Qty ?? 0,
                        Dispocode = item.Dispocode ?? "NA",
                        DisplayExchangeAble = display,
                        IsLastMinute = isLastMinute
                    });
                }
                return response;
            }

            return null;
        }
    }
}
