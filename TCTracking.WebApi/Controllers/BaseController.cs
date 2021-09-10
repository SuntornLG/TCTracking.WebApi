using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TCTracking.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly HttpContext _hcontext;
        private readonly ClaimsIdentity _userClaim;

        public BaseController(IHttpContextAccessor haccess)
        {
            _hcontext = haccess.HttpContext;
            _userClaim = _hcontext.User.Identity as ClaimsIdentity;
        }

        public string FullName { get { return _userClaim.FindFirst("FullName").Value; } }
        public string Email { get { return _userClaim.FindFirst("Email").Value; } }

        public string Role { get { return _userClaim.FindFirst(x => x.Type == ClaimTypes.Role).Value; } }
    }
}
