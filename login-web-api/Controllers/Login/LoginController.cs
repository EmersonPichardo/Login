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
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SecurityContext context;
        private readonly SesionConfiguration sesionConfiguration;
        private readonly HashingConfiguration hashingConfiguration;

        public LoginController(SecurityContext context, IOptions<SesionConfiguration> sesionConfiguration, IOptions<HashingConfiguration> hashingConfiguration)
        {
            this.context = context;
            this.sesionConfiguration = sesionConfiguration.Value;
            this.hashingConfiguration = hashingConfiguration.Value;
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

                int interationsBytes_Length = 2;
                byte[] interationsBytes = new byte[interationsBytes_Length];
                Buffer.BlockCopy(user.Salt, hashingConfiguration.SaltLength, interationsBytes, 0, interationsBytes_Length);

                int algorithmBytes_StartIndex = hashingConfiguration.SaltLength + interationsBytes_Length;
                byte[] algorithmBytes = new byte[user.Salt.Length - algorithmBytes_StartIndex];
                Buffer.BlockCopy(user.Salt, algorithmBytes_StartIndex, algorithmBytes, 0, algorithmBytes.Length);

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

                    if (!IsCurrentPolicy(iterations, algorithm, hashedPassword.Length)) UpdatePasswordPolicy(ref user, credentials.Password);

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

        private bool IsCurrentPolicy(short iterations, string algorithm, int passwordLength)
        {
            return (
                passwordLength == hashingConfiguration.AlgorithmLength &&
                algorithm == hashingConfiguration.Algorithm &&
                iterations == hashingConfiguration.Iterations
            );
        }

        private void UpdatePasswordPolicy(ref User user, string password)
        {
            (user.Password, user.Salt) = PasswordGenerator.Generate(password, hashingConfiguration);

            context.Entry(user).State = EntityState.Modified;
        }
    }
}
