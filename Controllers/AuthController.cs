using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using byteflow_server.DataAccess;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (loginRequest == null)
                {
                    return BadRequest("Login details are required");
                }

                // Check if the input is an email
                bool isEmail = loginRequest.UserNameOrEmail.Contains("@");

                // Query based on either username or email
                var user = await db.Users.FirstOrDefaultAsync(u => 
                    (isEmail ? u.Email == loginRequest.UserNameOrEmail : u.UserName == loginRequest.UserNameOrEmail));

                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                if (user.Password == loginRequest.Password)
                {
                    var token = GenerateToken(user);
                    return Ok(new { token });
                }

                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Rabiul-Islam-Rabi-ByteFlow-System"));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: "*",
                audience: "*",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

