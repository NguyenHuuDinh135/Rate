using Identity.API.Quickstar.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.API.Models;

namespace Identity.API.Quickstar.Account
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // =========================
        // GET: /Account/Login
        // =========================
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                EnableLocalLogin = true,
                AllowRememberLogin = true
            };

            return View(vm);
        }

        // =========================
        // POST: /Account/Login
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validate null trước khi dùng
            if (string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberLogin,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                    Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account is locked");
                return View(model);
            }

            ModelState.AddModelError("", "Login failed");
            return View(model);
        }
    }
}
