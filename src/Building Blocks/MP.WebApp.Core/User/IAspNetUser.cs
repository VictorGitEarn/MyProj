using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MP.WebApp.Core.User
{
    public interface IAspNetUser
    {
        string Name { get; }

        Guid GetUserById();
        
        string GetUserMail();
        
        string GetUserToken();
        
        string GetRefreshUserToken();
        
        bool IsAutheticated();
        
        IEnumerable<Claim> GetClaims();
        
        HttpContext GetHttpContext();
    }
}
