using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Interfaces;

namespace Task4.Pages;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserName { get; set; }
    }

    public void OnGet()
    {}

    public async Task<IActionResult> OnPostAsync()
    {
        if(!ModelState.IsValid)
            return Page();
        var result = await _userService.CreateUserAsync(Input.Email, Input.Password, Input.UserName);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Errors[0]);
            return Page();
        }
        return RedirectToPage("/Verification");
    }
}
