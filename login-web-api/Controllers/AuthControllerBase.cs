using login_data_access.Contexts.SecurityContext;
using login_web_api.SettingsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
        ) {
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
            Type type = typeof(T);

            if (type == typeof(string))
            {
                StringValues token;
                Request.Headers.TryGetValue(tokenName, out token);

                return (T)(object)token.ToString();
            }
            else if (type == typeof(Guid))
            {
                StringValues token;
                Request.Headers.TryGetValue(tokenName, out token);

                return (T)(object)Guid.Parse(token);
            }
            else if (type == typeof(byte[]))
            {
                StringValues token;
                Request.Headers.TryGetValue(tokenName, out token);

                return (T)(object)Guid.Parse(token).ToByteArray();
            }
            else
            {
                return default;
            }
        }
    }
}
