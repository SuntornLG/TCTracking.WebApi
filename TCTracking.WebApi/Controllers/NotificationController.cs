
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;
using TCTracking.WebApi.Configuration;

namespace TCTracking.WebApi.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService, IHttpContextAccessor haccess) : base(haccess)
        {
            this._notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _notificationService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _notificationService.GetByIdAsync(id));
        }

        [Authorize(Roles = "1, 2")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] NotificationRequest request)
        {
            try
            {
                request.CreatedBy = FullName;
                request.UpdatedBy = FullName;
                await _notificationService.AddAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(501, ex.Message.ToString());
            }

        }

        [Authorize(Roles = "1, 2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NotificationRequest request)
        {
            request.UpdatedBy = FullName;
            var result = await _notificationService.UpdateAsync(id, request);
            return Ok(result);
        }

        [Authorize(Roles = "1, 2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _notificationService.DeleteAsync(id, FullName);
            return Ok(result);
        }

    }
}
