using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using SmsWebSender.Models;
using SmsWebSender.ViewModels.Account;

namespace SmsWebSender.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                return RedirectToAction("Index","Sms");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(SmsController.Index), "Sms");
                    }
                }

                ModelState.AddModelError(string.Empty, "Innskráning mistókst");
                return View(vm);
            }

            // If we got this far, something failed, redisplay form
            return View(vm);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        [Route("notandi/stillingar")]
        public async Task<IActionResult> Settings()
        {
            var vm = new SettingsViewModel();
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            vm.SendSmsName = user.SendSmsName;
            vm.SmsTemplate = user.SmsTemplate;
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [Route("notandi/stillingar")]
        public async Task<IActionResult> Settings(SettingsViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            user.SendSmsName = vm.SendSmsName;
            user.SmsTemplate = vm.SmsTemplate;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index","Sms");
        }

        #region Helpers

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(SmsController.Index), "Sms");
            }
        }

        #endregion
    }
}
