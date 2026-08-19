using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Interfaces;
using Task4.Models;

namespace Task4.Pages;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(IUserService userService, SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _signIn = signIn;
        _userManager = userManager;
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task OnGetAsync()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User doesn't exist. Please, sign up");
            return Page();
        }
        if(user.Status == Status.blocked)
        {
            ModelState.AddModelError(string.Empty, "You have been blocked");
            return Page();
        }
        var result = await _signIn.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Incorrect login or password");
            return Page();
        }
        var timeResult = await _userService.UpdateLastLoginTimeAsync(Input.Email);
        if (!timeResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, timeResult.Errors[0]);
            return Page();
        }
        return RedirectToPage("/Index");
    }

    //endpoint to reset password
}
