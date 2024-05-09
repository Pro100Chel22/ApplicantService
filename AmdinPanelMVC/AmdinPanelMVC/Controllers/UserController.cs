﻿using Microsoft.AspNetCore.Mvc;
using AmdinPanelMVC.Models;
using AmdinPanelMVC.Services.Interfaces;
using AmdinPanelMVC.Helpers;
using AmdinPanelMVC.Filters;
using AmdinPanelMVC.DTOs;
using AmdinPanelMVC.Mappers;
using Common.Models.Models;
using Common.API.Helpers;

namespace AmdinPanelMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IAuthService _userService;

        public UserController(IAuthService userService)
        {
            _userService = userService;
        }

        [RequiredUnauthorize]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredUnauthorize]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            ExecutionResult<TokensResponseDTO> response = await _userService.LoginAsync(new()
            {
                Email = login.Email,
                Password = login.Password,
            });

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError("", error.Value[0]);
                }

                return View(login);
            }

            TokensResponseDTO loginResponse = response.Result!;
            HttpContext.Response.Cookies.SetTokens(loginResponse.JwtToken, loginResponse.RefreshToken);

            return Redirect("User/Profile");
        }

        [HttpGet]
        [RequiredAuthorize]
        public async Task<IActionResult> Profile()
        {
            if(!HttpContext.TryGetUserId(out Guid managerId))
            {
                return Redirect("/Error");
            }

            ExecutionResult<ManagerDTO> result = await _userService.GetManagerProfileAsync(managerId);
            if(!result.IsSuccess)
            {
                return Redirect("/Error");
            }

            return View(result.Result!.ToProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredAuthorize]
        public async Task<IActionResult> Email(ChangeEmailViewModel changeEmail)
        {
            return Redirect(changeEmail.Email);//View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredAuthorize]
        public async Task<IActionResult> FullName(ChangeFullNameViewModel changeFullName)
        {
            return Redirect(changeFullName.FullName);//View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredAuthorize]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.TryGetAccessTokenJTI(out Guid accessTokenJTI))
            {
                ExecutionResult result = await _userService.LogoutAsync(accessTokenJTI);

                if (result.IsSuccess)
                {
                    HttpContext.Response.Cookies.RemoveTokens();
                    return Redirect("Login");
                }

                if (result.Errors["LogoutFail"] is not null)
                {
                    HttpContext.Response.Cookies.RemoveTokens();
                }
            }
            
            return View("Login");
        }
    }
}
