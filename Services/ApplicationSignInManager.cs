using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task4.Models;

namespace Task4.Services;

public sealed class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation)
    : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    { }

    public override async Task<bool> CanSignInAsync(ApplicationUser user)
    {
        if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            //Logger.LogDebug(EventIds.UserCannotSignInWithoutConfirmedEmail, "User cannot sign in without a confirmed email.");
            return false;
        }
        if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
        {
            //Logger.LogDebug(EventIds.UserCannotSignInWithoutConfirmedPhoneNumber, "User cannot sign in without a confirmed phone number.");
            return false;
        }
        if (user.Status == Status.blocked)
            return false;
        return true;
    }
}
