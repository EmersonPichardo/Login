using login_data_access.Contexts.SecurityContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;

namespace login_web_api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateApplicationTokenAttribute : ActionFilterAttribute
    {
        private IConfiguration AppSetting { get; }

        public ValidateApplicationTokenAttribute()
        {
            AppSetting =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues applicationToken;
            if (!context.HttpContext.Request.Headers.TryGetValue("ApplicationToken", out applicationToken))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (string.IsNullOrWhiteSpace(applicationToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Guid applicationTokenGuid;
            if (!Guid.TryParse(applicationToken, out applicationTokenGuid))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool isRemote = AppSetting.GetValue<bool>("IsRemote");
            string connectionString = AppSetting.GetConnectionString($"SecurityConnection{(isRemote ? "Remote" : "")}");
            DbContextOptionsBuilder<SecurityContext> optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            SecurityContext dbContext = new SecurityContext(optionsBuilder.Options);
            if (!(dbContext.Applications.Any(app => app.Id == applicationToken.ToString() && app.Status == ApplicationStatus.Active.ToString())))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
