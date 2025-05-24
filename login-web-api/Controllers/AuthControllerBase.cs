using login_data_access.Contexts.SecurityContext;
using login_web_api.SettingsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;

namespace login_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController, ValidateApplicationToken]
    public class AuthControllerBase : ControllerBase
    {
        protected readonly SecurityContext context;
        protected readonly HashingConfiguration hashingConfiguration;
        protected readonly SesionConfiguration sesionConfiguration;

        public AuthControllerBase(
            SecurityContext context,
            IOptions<HashingConfiguration> hashingConfiguration = null,
            IOptions<SesionConfiguration> sesionConfiguration = null
        )
        {
            this.context = context;
            this.hashingConfiguration = hashingConfiguration.Value;
            this.sesionConfiguration = sesionConfiguration.Value;
        }

        protected T GetApplicationToken<T>()
        {
            return GetToken<T>("ApplicationToken");
        }

        protected T GetUserToken<T>()
        {
            return GetToken<T>("Token");
        }

        private T GetToken<T>(string tokenName)
        {
            Request.Headers.TryGetValue(tokenName, out var token);
            var type = typeof(T);

            return type == typeof(string) ? (T)(object)token.ToString() :
                   type == typeof(Guid) && Guid.TryParse(token, out var guid) ? (T)(object)guid :
                   type == typeof(byte[]) && Guid.TryParse(token, out guid) ? (T)(object)guid.ToByteArray() :
                   default;
        }
    }
}
