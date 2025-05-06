using byteflow_server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using byteflow_server.DataAccess;

namespace byteflow_server.Controllers
{

        [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ByteFlowDbContext db;

        public AuthController(ByteFlowDbContext dbContext)
        {
            db = dbContext;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] User details)
        {
            try
            {
                if (details == null)
                {
                    return BadRequest();
                }
                var user = db.Users.FirstOrDefault(user => user.UserName == details.UserName);
                if (user == null)
                {
                    return Unauthorized();
                }
                if (user.UserName == details.UserName && user.Password == details.Password )
                {
                    var token = GenerateToken(user);
                    return Ok(new { token });
                }
                else if (user.Email == details.Email && user.Password == details.Password)
                {
                    var token = GenerateToken(user);
                    return Ok(new { token });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
            return BadRequest();
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Rabiul-Islam-Rabi-ByteFlow-System"));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty), 
                new Claim(JwtRegisteredClaimNames.Jti, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: "*",
                audience: "*",
                claims: claim,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

