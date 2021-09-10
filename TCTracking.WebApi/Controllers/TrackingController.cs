
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TCTracking.Service.Constants;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;
using TCTracking.WebApi.Configuration;

namespace TCTracking.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : BaseController
    {
        private readonly ITCSService _iTCService;
        private readonly IFileManagerService _fileManagerService;
        private readonly INotificationService _notificationService;

        public TrackingController(ITCSService tCSService, IFileManagerService fileManager,
            INotificationService notificationService,

            IHttpContextAccessor haccess) : base(haccess)
        {
            _iTCService = tCSService;
            _fileManagerService = fileManager;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrackings()
        {
            var results = await _iTCService.GetTCsAsync();
            return Ok(results);
        }

        [HttpGet("{id}", Name = "GetTracking")]
        public async Task<IActionResult> GetTracking(string id)
        {
            return Ok(await _iTCService.GetTCAsync(id));
        }


        [HttpGet]
        [Route("reminder")]
        public async Task<IActionResult> Reminder()
        {

            int dayAfterImpl = Constain.DAY_AFTER_IMPLEMENT;
            int dayBeforeImpl = Constain.DAY_BEFORE_IMPLEMENT;
            var notification = await _notificationService.GetActiveItems();
            if (notification.Any())
            {
                dayAfterImpl = notification?.FirstOrDefault()?.NotificationAfterDateDay ?? 0;
                dayBeforeImpl = notification?.FirstOrDefault()?.NotificatonBeforeDateDay ?? 0;
            }


            var results = await _iTCService.GetRemider(dayBeforeImpl, dayAfterImpl);
            return Ok(results);
        }

        [HttpGet]
        [Route("notification")]
        public async Task<IActionResult> Notification()
        {
            int dayAfterImpl = Constain.DAY_AFTER_IMPLEMENT;
            int dayBeforeImpl = Constain.DAY_BEFORE_IMPLEMENT;

            var notification = await _notificationService.GetActiveItems();
            if (notification.Any())
            {
                dayAfterImpl = notification?.FirstOrDefault()?.NotificationAfterDateDay ?? 0;
                dayBeforeImpl = notification?.FirstOrDefault()?.NotificatonBeforeDateDay ?? 0;
            }

            var count = await _iTCService.GetCountRemider(dayBeforeImpl, dayAfterImpl);
            return Ok(new NotificationCount { Count = count });
        }

        [Authorize(Roles = "1, 2")]
        [HttpPost]
        public async Task<IActionResult> AddTracking([FromBody] TCSRequest request)
        {
            try
            {
                request.CreatedBy = FullName;
                request.UpdatedBy = FullName;
                request.Date = Convert.ToDateTime(request.DateString);
                var recordId = await _iTCService.AddAsync(request);
                if (request.Images.Count > 0)
                    foreach (var img in request.Images)
                        _fileManagerService.MoveFile(img, recordId);


                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(501, ex.Message.ToString());
            }
        }

        [Authorize(Roles = "1, 2")]
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            List<TrackingImages> images = new List<TrackingImages>();
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var files = formCollection.Files;
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var imageName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var fullPath = Path.Combine(pathToSave, imageName);
                            var dbPath = Path.Combine(folderName, imageName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                                file.CopyTo(stream);

                            images.Add(new TrackingImages { Image = imageName });
                        }
                    }

                    List<TrackingImages> resultImages = new List<TrackingImages>();

                    var oldImage = images.Where(x => x.Image.Trim().ToLower().Contains("old")).FirstOrDefault();
                    var newImage = images.Where(x => x.Image.Trim().ToLower().Contains("new")).FirstOrDefault();

                    resultImages.Add(oldImage);
                    resultImages.Add(newImage);

                    return Ok(resultImages);
                }
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Authorize(Roles = "1, 2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTracking(string id, [FromBody] TCSRequest request)
        {

            if ( await IsAllowOperation (id))
            {
                request.UpdatedBy = FullName;
                request.Date = Convert.ToDateTime(request.DateString);
                var result = await _iTCService.UpdateTCAsync(id, request);

                if (request.Images.Count > 0)
                    foreach (var img in request.Images)
                        _fileManagerService.MoveFile(img, id);

                return Ok(result);
            }
            else
            {
                return StatusCode(403, "Operation not allow");
            }

        }


        [Authorize(Roles = "1, 2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await IsAllowOperation(id))
            {
                var result = await _iTCService.DeleteAsync(id, FullName);
                return Ok(result);
            }
            else
            {
                return StatusCode(403, "Operation not allow");
            }

        }

        private async Task<bool> IsAllowOperation(string id)
        {
            var tc = await _iTCService.GetTCAsync(id);
            if (tc != null)
            {
                if (tc.IsCompleted)
                {
                    if (Role == "1")
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }
    }
}
