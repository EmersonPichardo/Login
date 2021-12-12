using login_data_access.Contexts.SecurityContext;
using login_data_access.Contexts.SecurityContext.Models;
using login_web_api.SettingsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace login_web_api.Controllers.Login
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SecurityContext context;
        private readonly SesionConfiguration sesionConfiguration;

        public LoginController(SecurityContext context, IOptions<SesionConfiguration> sesionConfiguration)
        {
            this.context = context;
            this.sesionConfiguration = sesionConfiguration.Value;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> IsTokenValid()
        {
            try
            {
                StringValues token;
                if (!Request.Headers.TryGetValue("Token", out token)) return BadRequest();

                if (string.IsNullOrWhiteSpace(token)) return BadRequest();

                Guid tokenGuid;
                if (!Guid.TryParse(token, out tokenGuid)) return BadRequest();

                Sesion sesion = await context.Sesions.FindAsync(tokenGuid.ToByteArray());
                if (sesion == null || sesion?.ValidUntil <= DateTime.Now) return Ok(false);

                sesion.ValidUntil = DateTime.Now.AddMinutes(sesionConfiguration.Minutes);
                context.Entry(sesion).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception exception)
            {
                return Problem(title: exception.Message, detail: exception.StackTrace);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserData>> Login(Credentials credentials)
        {
            try
            {
                if (credentials == null ||
                    string.IsNullOrWhiteSpace(credentials.Email) ||
                    string.IsNullOrWhiteSpace(credentials.Password)
                )
                    return BadRequest();

                User user = context.Users.SingleOrDefault(user => user.Email == credentials.Email);

                if (user == null) return NotFound();

                byte[] interationsBytes = new byte[2];
                byte[] algorithmBytes = new byte[user.Salt.Length - 18];

                Buffer.BlockCopy(user.Salt, 16, interationsBytes, 0, 2);
                Buffer.BlockCopy(user.Salt, 18, algorithmBytes, 0, algorithmBytes.Length);

                short iterations = BitConverter.ToInt16(interationsBytes);
                string algorithm = Encoding.UTF8.GetString(algorithmBytes);
                byte[] hashedPassword = new Rfc2898DeriveBytes(credentials.Password, user.Salt, iterations, new HashAlgorithmName(algorithm)).GetBytes(user.Password.Length);

                if (Enumerable.SequenceEqual(user.Password, hashedPassword))
                {
                    Sesion currentSesion = context.Sesions.SingleOrDefault(sesion => sesion.User_Id == user.Id);

                    if (currentSesion != null)
                    {
                        context.Remove(currentSesion);
                        await context.SaveChangesAsync();
                    }

                    currentSesion = new Sesion()
                    {
                        Token = new byte[16],
                        User_Id = user.Id,
                        ValidUntil = DateTime.Now.AddMinutes(sesionConfiguration.Minutes)
                    };

                    new RNGCryptoServiceProvider().GetBytes(currentSesion.Token);

                    await context.AddAsync(currentSesion);

                    UserData data = new UserData()
                    {
                        Name = user.Name,
                        Token = new Guid(currentSesion.Token).ToString()
                    };

                    await context.SaveChangesAsync();

                    return Ok(data);
                }

                return NotFound();

            }
            catch (Exception exception)
            {
                return Problem(title: exception.Message, detail: exception.StackTrace);
            }
        }
    }
}
