using System.Text;
using System.Threading.Tasks;
using Asp_.NET_MVC_Core_Mentoring_Module1.Models;
using Asp_.NET_MVC_Core_Mentoring_Module1.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("User").ConfigureAwait(false);

            return View(users);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Error");

            var user = CreateUser(model.Email);

            var result = await _userManager.CreateAsync(user, model.Password);

            await AssignRoleToUser(user, "User");

            if (result.Succeeded)
            {
                await SendConfirmationLink(user, "Activate");
                return View("ConfirmationLinkSent");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("error", error.Description);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return View("NotFound");
            }

            await SendConfirmationLink(user, "UpdatePassword");

            return View("ConfirmationLinkSent");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassword(string userId, string code)
        {
            var codeDecoded = DecodeUserCode(code);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, codeDecoded);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            var model = new RegisterViewModel
            {
                Email = user.Email
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(RegisterViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                return View("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("error", error.Description);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Activate(string userId, string code)
        {
            var codeDecoded = DecodeUserCode(code);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, codeDecoded);
            return View(result.Succeeded ? "RegistrationConfirmed" : "Error");
        }

        [HttpGet]
        public IActionResult LoginViaAzure(string returnUrl)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userName = model.Email;
            var password = model.Password;
            var isPersistent = model.RememberMe;
            const bool lockoutOnFailure = false;
            
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                await _signInManager.SignOutAsync();
            }

            return View("LoggedOut");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? (IActionResult)Redirect(returnUrl)
                : RedirectToAction("Index", "Home");
        }


        private async Task SendConfirmationLink(IdentityUser user, string action)
        {
            const string subject = "Account Confirmation";
            var email = user.Email;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var codeEncoded = EncodeUserCode(token);
            var callbackUrl = CreateCallbackUrl(user.Id, codeEncoded, action);

            var message = $"Please, confirm your account by clicking this link: + {callbackUrl}";

            await _emailService.SendEmailAsync(email, subject, message);
        }

        private string CreateCallbackUrl(string userId, string codeEncoded, string action)
        {
            var args = new { userId = userId, code = codeEncoded };
            var callbackUrl = Url.ActionLink(action, "Account", args, HttpContext.Request.Scheme);
            return callbackUrl;
        }

        private static string EncodeUserCode(string token)
        {
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            return codeEncoded;
        }


        private static string DecodeUserCode(string code)
        {
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            return codeDecoded;
        }

        private async Task AssignRoleToUser(IdentityUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);
        }

        private static IdentityUser CreateUser(string email)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };
            return user;
        }
    }
}