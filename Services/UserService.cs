using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.Dtos;
using Task4.Interfaces;
using Task4.Models;

namespace Task4.Services;

public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Result> CreateUserAsync(string email, string password, string userName)
    {
        var existingUser = await _userManager.FindByNameAsync(email);
        if (existingUser != null)
            return Result.Failure(new string[] { "User with such email already registrated" });
        var newUser = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = false
        };
        newUser.SetLoginTime(DateTime.Now);
        var result = await _userManager.CreateAsync(newUser, password);
        return Result.FromIdentity(result);
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
        if (userIds == null || userIds.Count == 0)
            return Result.Failure(new string[] { "Choose users to delete" });
        var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        if (users.Count == 0 || users == null)
            return Result.Failure(new string[] { "Selected users not found" });
        try
        {
            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(new string[] { "Failed to delete selected users" });
        }
    }

    public async Task<Result> DeleteUnverifiedUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] { "User not found" });
        if (user.Status != Status.unverified)
            return Result.Failure(new string[] { "This user already verified" });
        var result = await _userManager.DeleteAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> DeleteUnverifiedUserListAsync(List<string> userIds)
    {
        if (userIds == null || userIds.Count == 0)
            return Result.Failure(new string[] {"Choose unverified users to delete" });
        var users = await _context.Users.Where(u => userIds.Contains(u.Id) && u.Status == Status.unverified).ToListAsync();
        if (users == null || users.Count == 0)
            return Result.Failure(new string[] { "Failed to delete selected unverified users. Users already verified account" });
        try
        {
            _context.Users.RemoveRange(users);   
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(new string[] { "Failed to delete selected unverified users" });
        }
    }

    public async Task<List<ApplicationUserDto>> GetAllUsersAsync(string currentUserId)
    {
        var result = await _userManager.Users
                .Where(u => u.Id != currentUserId)
                .OrderByDescending(u => u.LastLoginTime)
                .Select(u => new ApplicationUserDto(u.UserName, u.Email, u.Status, u.LastLoginTime, u.Id))
                .ToListAsync();
        return result;
    }

    public async Task<ApplicationUserDto?> GetUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;
        return new ApplicationUserDto(user.UserName, user.Email, user.Status, user.LastLoginTime, userId);
    }

    public async Task<Result> BlockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] {"User not found"});
        if (user.Status == Status.blocked)
            return Result.Failure(new string[] {"User already blocked"});
        user.SetPreviousStatus();
        user.Block();
        await _userManager.UpdateSecurityStampAsync(user);
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> BlockUserListAsync(List<string> userIds)
    {
        if (userIds == null || userIds.Count == 0)
            return Result.Failure(new string[] { "Choose users to block" });
        var users = await _context.Users.Where(u => userIds.Contains(u.Id) && u.Status != Status.blocked).ToListAsync();
        if (users == null || users.Count == 0)
            return Result.Failure(new string[] { "Failed to block selected users. Users has been already blocked" });
        foreach(var user in users)
        {
            user.SetPreviousStatus();
            user.Block();
            user.SecurityStamp = Guid.NewGuid().ToString();
        }
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UnblockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(new string[] { "User not found" });
        if (user.Status != Status.blocked)
            return Result.Failure(new string[] { "Can't unblock active user" });
        user.Unblock();
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> UnblockUserListAsync(List<string> userIds)
    {
        if (userIds == null || userIds.Count == 0)
            return Result.Failure(new string[] { "Choose users to unblock" });
        var users = await _context.Users.Where(u => userIds.Contains(u.Id) && u.Status == Status.blocked).ToListAsync();
        if (users == null || users.Count == 0)
            return Result.Failure(new string[] { "Failed to unblock selected users. Users aren't blocked" });
        foreach (var user in users)
            user.Unblock();
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> VerifyUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure(new string[] { "User not found" });
        if (user.Status == Status.blocked)
            return Result.Failure(new string[] { "Can't verify blocked user" });
        if (user.Status == Status.active)
            return Result.Failure(new string[] { "User already verified account" });
        user.VerifyUser();
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> UpdateLastLoginTimeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure(new string[] {"User not found"});
        user.SetLoginTime(DateTime.UtcNow);
        var result = await _userManager.UpdateAsync(user);
        return Result.FromIdentity(result);
    }

    public async Task<Result> ResetUserPasswordAsync(string email, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure(new string[] {"Incorrect login"});
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return Result.FromIdentity(result);
    }
}
