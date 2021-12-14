using login_data_access.Contexts.SecurityContext;
using login_data_access.Contexts.SecurityContext.Models;
using login_web_api.SettingsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace login_web_api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SecurityContext context;
        private readonly HashingConfiguration hashingConfiguration;
        private readonly SesionConfiguration sesionConfiguration;

        public UserController(SecurityContext context, IOptions<HashingConfiguration> hashingConfiguration, IOptions<SesionConfiguration> sesionConfiguration)
        {
            this.context = context;
            this.hashingConfiguration = hashingConfiguration.Value;
            this.sesionConfiguration = sesionConfiguration.Value;
        }

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

                User user = context.Users.SingleOrDefault(user => user.Email == newUser.Email);

                if (user != null) return Conflict();

                user = new User()
                {
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

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            try
            {
                StringValues token;
                if (!Request.Headers.TryGetValue("Token", out token)) return BadRequest();

                if (string.IsNullOrWhiteSpace(token)) return BadRequest();

                Guid tokenGuid;
                if (!Guid.TryParse(token, out tokenGuid)) return BadRequest();

                Sesion sesion = await context.Sesions.FindAsync(tokenGuid.ToByteArray());
                if (sesion == null || sesion?.ValidUntil <= DateTime.Now) return NotFound();

                User user = await context.Users.FindAsync(sesion.User_Id);
                if (user == null) return NotFound();

                sesion.ValidUntil = DateTime.Now.AddMinutes(sesionConfiguration.Minutes);
                context.Entry(sesion).State = EntityState.Modified;
                await context.SaveChangesAsync();

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
