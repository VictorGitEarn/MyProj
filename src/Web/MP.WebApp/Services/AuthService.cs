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
    }

    public class AuthService : Service, IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            httpClient.BaseAddress = new Uri(settings.Value.AuthenticationUrl);

            _httpClient = httpClient;
        }

        public async Task<UserLoginResponse> Login(LoginUser usuarioLogin)
        {
            var loginContent = GetContent(usuarioLogin);

            var response = await _httpClient.PostAsync("/api/auth/authenticate", loginContent);

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

            var response = await _httpClient.PostAsync("/api/auth/new-account", registerContent);

            if (!HandleErrors(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }
    }
}