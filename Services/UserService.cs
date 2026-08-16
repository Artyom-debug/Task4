using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.Dtos;
using Task4.Interfaces;
using Task4.Models;

namespace Task4.Services;

public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(string, Result)> CreateUserAsync(string email, string password, string userName)
    {
        var existingUser = await _userManager.FindByNameAsync(email);
        if (existingUser != null)
            return (string.Empty, Result.Failure(new string[] { "User with such email already registrated" }));
        var newUser = new ApplicationUser
        { 
            UserName = userName,
            Email = email,
            EmailConfirmed = false
        };
        var result = await _userManager.CreateAsync(newUser, password);

        /*Email confirmation?*/

        return (newUser.Id, Result.FromIdentity(result));
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] { "User not found" });
        var result = await _userManager.DeleteAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> DeleteUserListAsync(List<string> userIds)
    {

    }

    public async Task<Result> DeleteUnverifiedUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] {"User not found"});
        if (user.Status != Status.unverified)
            return Result.Failure(new string[] { "This user already verified" });
        var result = await _userManager.DeleteAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> DeleteUnverifiedUserListAsync(List<string> userIds)
    {
        if (userIds.Count == 0)
            return Result.Failure(new string[] {"Choose unverified users to delete" });
        //foreach(var id in userIds)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //        return Result.Failure(new string[] { "User not found" });
        //    if (user.Status != Status.unverified)
        //        return Result.Failure(new string[] { $"User {user.Email} already verified" });

        //}
    }

    public async Task<List<ApplicationUserDto>> GetAllUsersAsync(string currentUserId)
    {
        var result = await _userManager.Users
            .Select(u => new ApplicationUserDto(u.UserName, u.Email, u.Status, u.LastLoginTime, u.Id, u.Id == currentUserId))
            .OrderByDescending(dto => dto.IsCurrentUser)
            .ToListAsync();
        return result;
    }

    //public async Task<ApplicationUserDto> GetUserAsync(string userId)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);
    //    return new ApplicationUserDto(user.UserName, user.Email, user.Status, user.LastLoginTime, userId, false);
    //}

    public async Task<Result> BlockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] {"User not found"});
        user.Block();
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> BlockUserListAsync(List<string> userIds)
    {

    }

    public async Task<Result> UnblockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] { "User not found" });
        user.Unblock(user.Status);
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> UnblockUserListAsync(List<string> userIds)
    {

    }

    public async Task<Result> VerifyUserAsync(string userId)
    {
        //var user = await _userManager.FindByIdAsync(userId);
        //if (user == null)
        //    return Result.Failure(new string[] { "User not found" });
        //user.VerifyUser();
        //var result = await _userManager.UpdateAsync(user);
        //return Result.FromIdentity(result);
    }
}
