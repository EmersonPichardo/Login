using login_data_access.Contexts.SecurityContext;
using login_data_access.Contexts.SecurityContext.Models;
using login_web_api.SettingsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace login_web_api.Controllers.Users
{
    public class UserController : AuthControllerBase
    {
        public UserController(SecurityContext context, IOptions<HashingConfiguration> hashingConfiguration, IOptions<SesionConfiguration> sesionConfiguration)
            : base(context, hashingConfiguration, sesionConfiguration) { }

        [HttpPost]
        public async Task<ActionResult> Register(NewUser newUser)
        {
            try
            {
                if (newUser == null ||
                    string.IsNullOrWhiteSpace(newUser.Email) ||
                    string.IsNullOrWhiteSpace(newUser.Name) ||
                    string.IsNullOrWhiteSpace(newUser.Password)
                )
                    return BadRequest();

                string applicationToken = GetApplicationToken<string>();

                User user = await context.Users.SingleOrDefaultAsync(user => user.Email == newUser.Email && user.Application_Id == applicationToken);
                if (user != null) return Conflict();

                user = new User()
                {
                    Application_Id = applicationToken,
                    Email = newUser.Email,
                    Name = newUser.Name
                };

                (user.Password, user.Salt) = PasswordGenerator.Generate(newUser.Password, hashingConfiguration);

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                return Created("", newUser);
            }
            catch (Exception exception)
            {
                return Problem(title: exception.Message, detail: exception.StackTrace);
            }
        }

        [HttpGet, ValidateUserToken]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            try
            {
                Sesion sesion = await context.Sesions.FindAsync(GetUserToken<byte[]>());
                if (sesion == null || sesion?.ValidUntil <= DateTime.Now) return NotFound();

                User user = await context.Users.FindAsync(sesion.User_Id);
                if (user == null) return NotFound();

                UserDto data = new UserDto()
                {
                    Email = user.Email,
                    Name = user.Name
                };

                return Ok(data);
            }
            catch (Exception exception)
            {
                return Problem(title: exception.Message, detail: exception.StackTrace);
            }
        }
    }
}
