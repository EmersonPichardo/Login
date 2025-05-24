using login_data_access.Contexts.SecurityContext;
using login_data_access.Contexts.SecurityContext.Models;
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
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateUserTokenAttribute : ActionFilterAttribute
    {
        private IConfiguration AppSetting { get; }

        public ValidateUserTokenAttribute()
        {
            AppSetting =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues token;
            if (!context.HttpContext.Request.Headers.TryGetValue("Token", out token))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Guid tokenGuid;
            if (!Guid.TryParse(token, out tokenGuid))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string connectionString = AppSetting.GetConnectionString("SecurityConnection");
            DbContextOptionsBuilder<SecurityContext> optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
            optionsBuilder.UseSqlServer(connectionString);

            SecurityContext dbContext = new SecurityContext(optionsBuilder.Options);

            Sesion sesion = dbContext.Sesions.Find(tokenGuid.ToByteArray());
            if (sesion == null || sesion?.ValidUntil <= DateTime.Now)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!dbContext.Users.Any(user => user.Id == sesion.User_Id))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            sesion.ValidUntil = DateTime.Now.AddMinutes(AppSetting.GetValue<int>("Sesion:Minutes"));
            dbContext.Entry(sesion).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}
