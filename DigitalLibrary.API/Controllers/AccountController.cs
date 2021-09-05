using System.Linq;
using System.Threading.Tasks;
using DigitalLibrary.API.Services.MailService;
using DigitalLibrary.Models.AccountModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DigitalLibrary.API.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        private readonly IStringLocalizer<CustomIdentityResource> _localizer;
        private readonly IMailService _mailService;

        public AccountController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService interactionService,
            IStringLocalizer<CustomIdentityResource> localizer,
            IMailService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
            _localizer = localizer;
            _mailService = mailService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl, string message)
        {
            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            ViewBag.ReturnUrl = vm.ReturnUrl;
            ViewBag.Message = message;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            ViewBag.ReturnUrl = vm.ReturnUrl;

            if (ModelState.IsValid == false)
            {
                return View();
            }

            IdentityUser user = null;
            SignInResult result;

            // check if credential is email
            if (vm.EmailOrLogin.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(vm.EmailOrLogin);
            }

            // Log in if it is email
            if (user != null)
            {
                result = await _signInManager.PasswordSignInAsync(user, vm.Password, true, false);
            }
            else // Log in if it is username
            {
                result = await _signInManager.PasswordSignInAsync(vm.EmailOrLogin, vm.Password, true, false);
            }

            if (result.Succeeded)
            {
                return Redirect(vm.ReturnUrl);
            }

            if (result.IsNotAllowed)
            {
                ViewBag.Error = _localizer["NotAllowed"];
                return View();
            }

            if (result.IsLockedOut)
            {
                ViewBag.Error = _localizer["LockedOut"];
                return View();
            }

            ViewBag.Error = _localizer["Failed"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutContext = await _interactionService.GetLogoutContextAsync(logoutId);
            var logoutUri = logoutContext.PostLogoutRedirectUri;
            if (string.IsNullOrEmpty(logoutUri))
            {
                return BadRequest("ReturnUrl should be specified");
            }

            return Redirect(logoutUri);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest("ReturnUrl should be specified");
            }

            var vm = new RegistrationViewModel
            {
                ReturnUrl = returnUrl
            };

            ViewBag.ReturnUrl = returnUrl;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel vm)
        {
            ViewBag.ReturnUrl = vm.ReturnUrl;

            if (ModelState.IsValid == false)
            {
                return View();
            }

            var newUser = new IdentityUser
            {
                Email = vm.Email,
                UserName = vm.Username,
            };

            var result = await _userManager.CreateAsync(newUser, vm.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var message = Url.Action("ConfirmEmail", "Account", new {token, vm.Email},
                    HttpContext.Request.Protocol.Replace("/2", "S"), Request.Host.Value);
                await _mailService.SendEmailAsync(new MailRequest
                {
                    Body = string.Format(_localizer["ConfirmEmailLink"], message), //$"Confirm email link: {message}",
                    Subject = _localizer["Email confirmation"],
                    ToEmail = vm.Email
                });

                return RedirectToAction("Login", new
                {
                    returnUrl = vm.ReturnUrl,
                    message = _localizer["ConfirmationEmailSent"]
                });
            }

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "LoginAlreadyAssociated":
                        case "InvalidUserName":
                        case "DuplicateUserName":
                            ModelState.AddModelError("Username", error.Description);
                            break;
                        case "InvalidEmail":
                        case "DuplicateEmail":
                            ModelState.AddModelError("Email", error.Description);
                            break;
                        case "PasswordTooShort":
                        case "PasswordRequiresNonAlphanumeric":
                        case "PasswordRequiresDigit":
                        case "PasswordRequiresLower":
                        case "PasswordRequiresUpper":
                            ModelState.AddModelError("Password", error.Description);
                            break;
                        default:
                            ModelState.AddModelError("default", error.Description);
                            break;
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult PasswordRestoreRequest(string returnUrl)
        {
            var vm = new PasswordRestoreModel
            {
                ReturnUrl = returnUrl
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRestoreRequest(PasswordRestoreModel vm)
        {
            if (vm.Email == null)
            {
                ViewBag.Error = _localizer["Required"];
                return View();
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var message = Url.Action("PasswordRestore", "Account", new {token, vm.Email},
                    HttpContext.Request.Protocol.Replace("/2", "S"), Request.Host.Value);
                await _mailService.SendEmailAsync(new MailRequest
                {
                    Body = string.Format(_localizer["RestoreLink"],
                        message),
                    Subject = _localizer["Password Restore"],
                    ToEmail = vm.Email
                });

                return RedirectToAction("Login", new
                {
                    returnUrl = vm.ReturnUrl,
                    message = _localizer["PasswordResetEmailSent"]
                });
            }

            @ViewBag.Error = _localizer["Email is invalid or not found."];
            return View();
        }

        [HttpGet]
        public IActionResult PasswordRestore(string token, string email)
        {
            var vm = new PasswordRestoreModel
            {
                Token = token,
                Email = email
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRestore(PasswordRestoreModel vm)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResponseMessage", new
                    {
                        message = _localizer["PasswordChanged"]
                    });
                }

                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "PasswordTooShort":
                        case "PasswordRequiresNonAlphanumeric":
                        case "PasswordRequiresDigit":
                        case "PasswordRequiresLower":
                        case "PasswordRequiresUpper":
                            ModelState.AddModelError("Password", error.Description);
                            break;
                    }
                }

                return View();
            }

            return BadRequest("Error happened");
        }

        [HttpGet]
        public IActionResult ResponseMessage(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest();
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("ResponseMessage", new {message = _localizer["EmailConfirmed"]});
            }

            return BadRequest();
        }
    }
}
