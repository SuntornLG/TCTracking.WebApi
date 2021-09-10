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
    public class UsersService : IUsersService
    {
        private readonly IMongoCollection<Users> _userRepository;
        private readonly IPasswordGeneratorService _passwordService;
        public UsersService(IDbClient dbClient, IPasswordGeneratorService passwordService)
        {
            _userRepository = dbClient.GetUsersCollections();
            _passwordService = passwordService;
        }
        public async Task AddAsync(UserRequest user)
        {
            var model = Convert(user);
            model.CreatedDate = DateTime.Now;
            model.UpdatedDate = DateTime.Now;
            var password = user.Email;// _passwordService.GetRandomPassword();
            model.Password = _passwordService.Encrypt(user.Email, password);

            await _userRepository.InsertOneAsync(model);
        }

        public async Task<UsersResponse> GetUserAsync(string id)
        {
            var model = await _userRepository.Find(x => x.Id == id).FirstOrDefaultAsync();
            return GetUserResponse(new List<Users> { model }).FirstOrDefault();
        }

        public async Task<List<UsersResponse>> GetUsersAsync()
        {
            var models = await _userRepository.Find(x => true)
                .ToListAsync();

            var response = GetUserResponse(models);
            return response.OrderByDescending(o => o.UpdatedDate).ToList();
        }

        private List<UsersResponse> GetUserResponse(List<Users> models)
        {
            var modelList = new List<UsersResponse>();


            foreach (var model in models)
            {
                string userRoleDisplay = "ReadOnly";
                if (model.UserRole == "1")
                    userRoleDisplay = "Administrator";
                else if (model.UserRole == "2")
                    userRoleDisplay = "SuperUser";

                var response = new UsersResponse
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsActive = (model.IsActive == true) ? model.IsActive : false,
                    IsSystemAdmin = (model.IsSystemAdmin == true) ? model.IsSystemAdmin : false,
                    UserRole = model.UserRole,
                    CreatedBy = model.CreatedBy ?? string.Empty,
                    UpdatedBy = model.UpdatedBy ?? string.Empty,
                    UpdatedDate = model.UpdatedDate ?? DateTime.Now,
                    CreatedDate = model.CreatedDate ?? DateTime.Now,
                    UserRoleDisplay = userRoleDisplay
                };
                modelList.Add(response);

            }
            return modelList;

        }

        private Users Convert(UserRequest user)
        {
            bool isSystemAdmin = false;
            if (user.UserRole == "1")
                isSystemAdmin = true;

            var model = new Users
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UpdatedDate = DateTime.Now,
                IsActive = user.IsActive,
                IsSystemAdmin = isSystemAdmin,
                UpdatedBy = user.UpdatedBy,
                UserRole = user.UserRole
            };
            return model;

        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                await _userRepository.DeleteOneAsync(x => x.Id == id);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public async Task<bool> UpdateUserAsync(string id, UserRequest user)
        {
            bool isSystemAdmin = false;
            if (user.UserRole == "1")
                isSystemAdmin = true;

            try
            {
                var existUser = await _userRepository
                    .Find(x => x.Id == id).FirstOrDefaultAsync();

                if (existUser != null)
                {
                    var model = new Users
                    {
                        Id = id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsActive = user.IsActive,
                        IsSystemAdmin = isSystemAdmin,
                        UpdatedBy = user.UpdatedBy,
                        UpdatedDate = DateTime.Now,
                        Password = existUser.Password,
                        Email = user.Email,
                        UserRole = user.UserRole
                    };
                    var result = await _userRepository.ReplaceOneAsync(x => x.Id == id, model);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            try
            {
                var model = await _userRepository.Find(x => x.IsActive.Value && x.Email == email).FirstOrDefaultAsync();
                return model;
            }
            catch (Exception) { }
            return null;
        }

        public async Task<ChangePasswordResult> ChangePassword(Password password)
        {
            ChangePasswordResult result = new ChangePasswordResult();

            var userModel = await GetUserByEmail(password.Email);
            if (userModel == null)
            {
                result.ErrorMessage = "User is invalid!";
                result.ErrorCode = "500";
                result.IsSuccess = false;
                return result;
            }

            var isSamePassword = _passwordService.CheckPassword(password.Email, userModel.Password, password.OldPassword);
            if (!isSamePassword)
            {
                result.ErrorMessage = "Your password is not match";
                result.ErrorCode = "500";
                result.IsSuccess = false;
                return result;
            }

            try
            {
                var model = await _userRepository.Find(x => x.Email == password.Email).FirstOrDefaultAsync();
                var newPass = _passwordService.Encrypt(password.Email, password.NewPassword);
                model.Password = newPass;
                model.UpdatedDate = DateTime.Now.ToLocalTime();
                await _userRepository.ReplaceOneAsync(x => x.Email == password.Email, model);

                result.ErrorMessage = "Your password has been changed";
                result.ErrorCode = "";
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message.ToString();
                result.ErrorCode = "500";
                result.IsSuccess = false;
                return result;
            }

        }
    }
}
