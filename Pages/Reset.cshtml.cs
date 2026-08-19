using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Interfaces;

namespace Task4.Pages
{
    public class ResetModel : PageModel
    {
        private readonly IUserService _userService;

        public ResetModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            var result = await _userService.ResetUserPasswordAsync(Email, NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Errors[0]);
                return Page();
            }
            return RedirectToPage("/Index");
        }
    }
}
