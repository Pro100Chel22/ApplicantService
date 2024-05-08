﻿using Microsoft.AspNetCore.Identity;
using UserService.Core.Application.DTOs;
using UserService.Core.Application.Interfaces;
using UserService.Core.Domain.Entities;
using UserService.Infrastructure.Identity.Extensions;
using Common.Models.Enums;
using Common.Models.Models;

namespace UserService.Infrastructure.Identity.Services
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly ITokenDbService _tokenDbService;
        private readonly TokenHelperService _tokenHelperService;
        private readonly IServiceBusProvider _serviceBusProvider;

        public AuthService(UserManager<CustomUser> userManager, ITokenDbService tokenDbService, TokenHelperService tokenHelperService, SignInManager<CustomUser> signInManager, IServiceBusProvider serviceBusProvider)
        {
            _userManager = userManager;
            _tokenDbService = tokenDbService;
            _tokenHelperService = tokenHelperService;
            _signInManager = signInManager;
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task<ExecutionResult<TokenResponse>> ApplicantRegistrationAsync(RegistrationDTO registrationDTO)
        {
            CustomUser user = new()
            {
                FullName = registrationDTO.FullName,
                Email = registrationDTO.Email,
                UserName = $"{registrationDTO.FullName}_{Guid.NewGuid()}",
            };

            IdentityResult creationResult = await _userManager.CreateAsync(user, registrationDTO.Password);
            if (!creationResult.Succeeded) return creationResult.ToExecutionResultError<TokenResponse>();

            IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, Role.Applicant);
            if (!addingRoleResult.Succeeded) return addingRoleResult.ToExecutionResultError<TokenResponse>();

            ExecutionResult<TokenResponse> creatingTokenResult = await GetTokensAsync(user);
            if (!creatingTokenResult.IsSuccess) return creatingTokenResult;

            ExecutionResult sendingResult = await _serviceBusProvider.Notification.CreatedApplicantAsync(new User()
            {
                Id = Guid.Parse(user.Id),
                FullName = user.FullName,
                Email = user.Email,
            });
            if (!sendingResult.IsSuccess)
            {
                return new() { Errors = sendingResult.Errors };
            }

            return creatingTokenResult;
        }

        public async Task<ExecutionResult<ManagerLoggedinResponse>> ManagerLoginAsync(LoginDTO loginDTO)
        {
            ExecutionResult<CustomUser> result = await LoginAsync(loginDTO, [Role.Manager, Role.MainManager]);
            if (!result.IsSuccess) return new() { Errors = result.Errors };
            CustomUser user = result.Result!;

            return new()
            {
                Result = new()
                {
                    Id = Guid.Parse(user.Id),
                    Email = user.Email!,
                    FullName = user.FullName,
                }
            };
        }

        public async Task<ExecutionResult<TokenResponse>> ApplicantLoginAsync(LoginDTO loginDTO)
        {
            ExecutionResult<CustomUser> result = await LoginAsync(loginDTO, [Role.Manager, Role.MainManager]);
            if (!result.IsSuccess) return new() { Errors = result.Errors };
            CustomUser user = result.Result!;

            return await GetTokensAsync(user);
        }

        private async Task<ExecutionResult<CustomUser>> LoginAsync(LoginDTO loginDTO, string[] loginFor)
        {
            CustomUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return new(keyError: "LoginFail", error: "Invalid email or password.");
            }

            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!signInResult.Succeeded)
            {
                return new(keyError: "LoginFail", error: "Invalid email or password.");
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            if (!UserHaveRole(userRoles, loginFor))
            {
                return new(keyError: "LoginFail", error: "Invalid email or password.");
            }

            return new() { Result = user };
        }

        private bool UserHaveRole(IList<string> userRoles, string[] haveAnyRole)
        {
            foreach (string role in haveAnyRole)
            {
                if (userRoles.Contains(role)) return true;
            }
            return false;
        }

        public async Task<ExecutionResult> LogoutAsync(Guid accessTokenJTI)
        {
            bool result = await _tokenDbService.RemoveTokensAsync(accessTokenJTI);
            if (!result)
            {
                return new(keyError: "LogoutFail", error: "The tokens have already been deleted.");
            }
            return new(true);
        }

        public async Task<ExecutionResult<TokenResponse>> UpdateAccessTokenAsync(string refresh, Guid accessTokenJTI, Guid userId)
        {
            bool tokenExist = await _tokenDbService.TokensExist(refresh, accessTokenJTI);
            if (!tokenExist)
            {
                return new(keyError: "UpdateAccessTokenFail", error: "Tokens are not valid!");
            }

            bool removeResult = await _tokenDbService.RemoveTokensAsync(accessTokenJTI);
            if (!removeResult)
            {
                return new(keyError: "UpdateAccessTokenFail", error: "Unknow error");
            }

            CustomUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new(keyError: "UpdateAccessTokenFail", error: "Unknow error");
            }

            return await GetTokensAsync(user);
        }

        private async Task<ExecutionResult<TokenResponse>> GetTokensAsync(CustomUser user)
        {
            (string accessToken, Guid tokenJTI) = await _tokenHelperService.GenerateJWTTokenAsync(user);
            string refreshToken = _tokenHelperService.GenerateRefreshToken();

            bool saveTokenResult = await _tokenDbService.SaveTokensAsync(refreshToken, tokenJTI);
            if (!saveTokenResult)
            {
                return new(keyError: "UnknowError", error: "Unknown error");
            }

            return new()
            {
                Result = new TokenResponse()
                {
                    Access = accessToken,
                    Refresh = refreshToken,
                }
            };
        }
    }
}