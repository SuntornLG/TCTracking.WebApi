
using System.Collections.Generic;
using System.Threading.Tasks;
using TCTracking.Core.Models;
using TCTracking.Service.Dtos;

namespace TCTracking.Service.Interface
{
    public interface IUsersService
    {
        Task<List<UsersResponse>> GetUsersAsync();
        Task AddAsync(UserRequest user);
        Task<UsersResponse> GetUserAsync(string id);

        Task<bool> DeleteUserAsync(string id);

        Task<bool> UpdateUserAsync(string id, UserRequest user);

        Task<Users> GetUserByEmail(string email);

        Task<ChangePasswordResult> ChangePassword(Password password);


    }
}
