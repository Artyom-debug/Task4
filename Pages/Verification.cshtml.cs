using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task4.Interfaces;

namespace Task4.Pages;

public class VerificationModel : PageModel
{
    private readonly IUserService _userService;

    public VerificationModel(IUserService userService)
    {
        this._userService = userService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost([FromRoute]string email)
    {
        var result = await _userService.VerifyUserAsync(email);
        if(!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Errors[0]);
            return Page();
        }
        return RedirectToPage("/Index");
    }
}
