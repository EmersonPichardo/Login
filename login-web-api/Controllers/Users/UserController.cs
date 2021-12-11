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

namespace login_web_api.Controllers.Users
{
    [Route("api/user")]
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

                byte[] pureSalt = new byte[hashingConfiguration.SaltLength];
                byte[] interations = BitConverter.GetBytes(hashingConfiguration.Iterations);
                byte[] algorithm = Encoding.UTF8.GetBytes(hashingConfiguration.Algorithm);

                new RNGCryptoServiceProvider().GetNonZeroBytes(pureSalt);

                user.Salt = new byte[hashingConfiguration.SaltLength + interations.Length + algorithm.Length];
                Buffer.BlockCopy(pureSalt, 0, user.Salt, 0, pureSalt.Length);
                Buffer.BlockCopy(interations, 0, user.Salt, pureSalt.Length, interations.Length);
                Buffer.BlockCopy(algorithm, 0, user.Salt, pureSalt.Length + interations.Length, algorithm.Length);

                user.Password = new Rfc2898DeriveBytes(newUser.Password, user.Salt, hashingConfiguration.Iterations, new HashAlgorithmName(hashingConfiguration.Algorithm)).GetBytes(hashingConfiguration.AlgorithmLength);

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                return Created("", newUser);
            }
            catch (Exception exception)
            {
                return Problem(title: exception.Message, detail: exception.StackTrace);
            }
        }
    }
}
