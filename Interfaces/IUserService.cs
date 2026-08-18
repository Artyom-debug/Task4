using Task4.Dtos;
using Task4.Models;

namespace Task4.Interfaces;

public interface IUserService
{
    Task<Result> CreateUserAsync(string email, string password, string userName);

    Task<Result> DeleteUserAsync(string userId);

    Task<Result> DeleteUserListAsync(List<string> userIds);

    Task<Result> DeleteUnverifiedUserAsync(string userId);

    Task<Result> DeleteUnverifiedUserListAsync(List<string> userIds);

    Task<List<ApplicationUserDto>> GetAllUsersAsync(string currentUserId);

    Task<ApplicationUserDto> GetUserAsync(string userId);

    Task<Result> BlockUserAsync(string userId);

    Task<Result> BlockUserListAsync(List<string> userIds);

    Task<Result> UnblockUserAsync(string userId);

    Task<Result> UnblockUserListAsync(List<string> userIds);

    Task<Result> VerifyUserAsync(string userId);

    Task<Result> UpdateLastLoginTimeAsync(string email);  
}
