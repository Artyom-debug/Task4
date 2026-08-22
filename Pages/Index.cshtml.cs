using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task4.Models;

namespace Task4.Pages;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Status? UserStatus { get; set; }
    public string? Email { get; set; }

    public async Task OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
            return;
        var user = await _userManager.GetUserAsync(User);
        UserStatus = user?.Status;
        Email = user?.Email;
    }
}
