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

    public ApplicationUserDto CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            ModelState.AddModelError(string.Empty, "Current user not found");
            return NotFound(ModelState);
        }
        var currentUser = await _userService.GetUserAsync(currentUserId);
        if (currentUser == null)
        {
            ModelState.AddModelError(string.Empty, "Current user not found");
            return NotFound(ModelState);
        }
        CurrentUser = currentUser;
        var userList = await _userService.GetAllUsersAsync(currentUserId);
        AllUsers = userList;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        var operationResult = await _userService.DeleteUserAsync(userId);
        if(!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.DeleteUserListAsync(selectedIds);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteUnverifiedAsync(string userId)
    {
        var operationResult = await _userService.DeleteUnverifiedUserAsync(userId);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteSelectedUnverifiedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.DeleteUnverifiedUserListAsync(selectedIds);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostBlockUserAsync(string userId)
    {
        var operationResult = await _userService.BlockUserAsync(userId);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostBlockSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.BlockUserListAsync(selectedIds);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostUnblockUserAsync(string userId)
    {
        var operationResult = await _userService.UnblockUserAsync(userId);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }

    public async Task<IActionResult> OnPostUnblockSelectedUsersAsync(List<string> selectedIds)
    {
        var operationResult = await _userService.UnblockUserListAsync(selectedIds);
        if (!operationResult.Succeeded)
            ModelState.AddModelError(string.Empty, operationResult.Errors.First());
        return Page();
    }
}
