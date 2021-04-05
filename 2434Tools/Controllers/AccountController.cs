using _2434ToolsUser.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace _2434Tools.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public async Task<IActionResult> Login(String returnUrl = null)
        {
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(_2434Tools.Models.ViewModels.SingInViewModel model, String returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Adding an exclamation mark at the end to bypass ASP.NET password requirements
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var signIn = await _signInManager.PasswordSignInAsync(user, model.Password + "!", model.Remember, false);
                    if (signIn.Succeeded)
                    {
                        return this.Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
                    }
                }
                ModelState.AddModelError("", "User not found");
            }
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated) this.RedirectToAction("Login");
            await _signInManager.SignOutAsync();
            return this.Redirect("/");
        }
    }
}
