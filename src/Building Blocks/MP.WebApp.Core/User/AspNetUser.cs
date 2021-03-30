using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MP.WebApp.Core.User
{
    public class AspNetUser : IAspNetUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Name => _accessor.HttpContext.User.Identity.Name;

        public IEnumerable<Claim> GetClaims()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public HttpContext GetHttpContext()
        {
            return _accessor.HttpContext;
        }

        public Guid GetUserById()
        {
            return IsAutheticated() ? Guid.Parse(_accessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }

        public string GetUserMail()
        {
            return IsAutheticated() ? _accessor.HttpContext.User.GetUserMail() : "";
        }

        public string GetRefreshUserToken()
        {
            return IsAutheticated() ? _accessor.HttpContext.User.GetUserRefreshToken() : "";
        }

        public string GetUserToken()
        {
            return IsAutheticated() ? _accessor.HttpContext.User.GetUserToken() : "";
        }

        public bool IsAutheticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
