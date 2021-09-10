
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TCTracking.Service.Dtos;
using TCTracking.Service.Interface;

namespace TCTracking.WebApi.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService, IPasswordGeneratorService passwordService, IHttpContextAccessor haccess) : base(haccess)
        {
            _usersService = usersService;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var results = await _usersService.GetUsersAsync();
            return Ok(results);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _usersService.GetUserAsync(id);
            return Ok(result);
        }


        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _usersService.DeleteUserAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserRequest user)
        {
            user.UpdatedBy = FullName;
            var result = await _usersService.UpdateUserAsync(id, user);
            return Ok(result);
        }


        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword(Password password)
        {

            var result = await _usersService.ChangePassword(password);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserRequest user)
        {
            user.CreatedBy = FullName;
            user.UpdatedBy = FullName;

            var isUserExist = await _usersService.GetUserByEmail(user.Email);
            if (isUserExist != null)
            {
                var error = new ErrorCollection
                {
                    ErrorCode = "500",
                    ErrorMessage = "This user already exist",
                    IsSuccess = false
                };

                return BadRequest(error);
            }
            else
            {

                await _usersService.AddAsync(user);
                return Ok(user);
            }

        }


        [HttpGet]
        [Route("GetUserClaims")]
        public AccountModel GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            try
            {
                var user = identityClaims.FindFirst("FullName").Value;
                var email = identityClaims.FindFirst("Email").Value;
                var isSystemAdmin = Convert.ToBoolean(identityClaims.FindFirst("IsSystemAdmin").Value);
                var userRole = identityClaims.FindFirst(x => x.Type == ClaimTypes.Role)?.Value ?? "3";

                var claimsModel = new AccountModel
                {
                    Email = email,
                    IsSystemAdmin = isSystemAdmin,
                    FullName = user,
                    Role = userRole
                };
                return claimsModel;

            }
            catch (Exception ex) { }

            return null;
        }


    }
}
