using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Task4.Dtos;
using Task4.Interfaces;

namespace Task4.Pages;

public class AdminPanelModel : PageModel
{
    private readonly IUserService _userService;

    public AdminPanelModel(IUserService userService)
    {
        _userService = userService;
    }

    public List<ApplicationUserDto> AllUsers { get; set; } = new();

    public ApplicationUserDto? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadPageAsync();
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        var operationResult = await _userService.DeleteUserAsync(userId);
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.DeleteUserListAsync(selectedIds ?? new());
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteUnverifiedAsync(string userId)
    {
        var operationResult = await _userService.DeleteUnverifiedUserAsync(userId);
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteSelectedUnverifiedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.DeleteUnverifiedUserListAsync(selectedIds ?? new());
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBlockUserAsync(string userId)
    {
        var operationResult = await _userService.BlockUserAsync(userId);
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBlockSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.BlockUserListAsync(selectedIds ?? new());
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnblockUserAsync(string userId)
    {
        var operationResult = await _userService.UnblockUserAsync(userId);
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnblockSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.UnblockUserListAsync(selectedIds ?? new());
        if (!operationResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
            return await LoadPageAsync();
        }
        return RedirectToPage();
    }

    private async Task<IActionResult> LoadPageAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return RedirectToPage("/Index");

        var currentUser = await _userService.GetUserAsync(currentUserId);
        if (currentUser == null)
        {
            ModelState.AddModelError(string.Empty, "Current user not found");
            return Page();
        }

        CurrentUser = WithActivity(currentUser);
        AllUsers = (await _userService.GetAllUsersAsync(currentUserId))
            .Select(WithActivity)
            .ToList();
        return Page();
    }

    private static ApplicationUserDto WithActivity(ApplicationUserDto user)
        => user with { LastActivity = FormatLastActivity(user.LastLogin) };

    private static string FormatLastActivity(DateTime lastLogin)
    {
        var span = DateTime.UtcNow - lastLogin;
        if (span.TotalMinutes < 1) return "just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} minutes ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
        if (span.TotalDays < 7) return $"{(int)span.TotalDays} days ago";
        if (span.TotalDays < 28) return $"{(int)(span.TotalDays / 7)} weeks ago";
        return $"{(int)(span.TotalDays / 30)} months ago";
    }
}
