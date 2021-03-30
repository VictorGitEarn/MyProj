using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MP.Core.Communication;
using MP.WebApp.Core.User;
using MP.WebApp.Extentions;
using MP.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MP.WebApp.Services
{
    public interface IAuthService
    {
        Task<UserLoginResponse> Login(LoginUser usuarioLogin);

        Task<UserLoginResponse> Register(RegisterUser usuarioRegistro);

        Task ToLogin(UserLoginResponse resposta);

        Task Logout();

        bool TokenExpired();

        Task<bool> RefreshValidToken();
    }

    public class AuthService : Service, IAuthService
    {
        private readonly HttpClient _httpClient;

        private readonly IAspNetUser _user;
        private readonly IAuthenticationService _authenticationService;
        public AuthService(HttpClient httpClient,
                                   IOptions<AppSettings> settings,
                                   IAspNetUser user,
                                   IAuthenticationService authenticationService)
        {
            httpClient.BaseAddress = new Uri(settings.Value.AuthenticationUrl);

            _httpClient = httpClient;
            _user = user;
            _authenticationService = authenticationService;
        }

        public async Task<UserLoginResponse> Login(LoginUser usuarioLogin)
        {
            var loginContent = GetContent(usuarioLogin);

            var response = await _httpClient.PostAsync("/api/authentication/authenticate", loginContent);

            if (!HandleErrors(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }

        public async Task Logout()
        {
            await _authenticationService.SignOutAsync(
                _user.GetHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                null);
        }

        public async Task<bool> RefreshValidToken()
        {
            var response = await UseRefreshToken(_user.GetRefreshUserToken());

            if (response.AccessToken != null && response.ResponseResult == null)
            {
                await ToLogin(response);
                return true;
            }

            return false;
        }

        public async Task<UserLoginResponse> UseRefreshToken(string refreshToken)
        {
            var refreshTokenContent = GetContent(refreshToken);

            var response = await _httpClient.PostAsync("/api/authentication/refresh-token", refreshTokenContent);

            if (!HandleErrors(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }

        public async Task<UserLoginResponse> Register(RegisterUser usuarioRegistro)
        {
            var registerContent = GetContent(usuarioRegistro);

            var response = await _httpClient.PostAsync("/api/authentication/new-account", registerContent);

            if (!HandleErrors(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }

        public bool TokenExpired()
        {
            var jwt = _user.GetUserToken();
            if (jwt is null) return false;

            var token = GetFormatedToken(jwt);
            return token.ValidTo.ToLocalTime() < DateTime.Now;
        }

        public async Task ToLogin(UserLoginResponse resposta)
        {
            var token = GetFormatedToken(resposta.AccessToken);

            var claims = new List<Claim>();
            claims.Add(new Claim("JWT", resposta.AccessToken));
            claims.Add(new Claim("RefreshToken", resposta.RefreshToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true
            };

            await _authenticationService.SignInAsync(
                _user.GetHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public static JwtSecurityToken GetFormatedToken(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }
    }
}