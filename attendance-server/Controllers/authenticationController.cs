using attendance_server.Model;
using attendance_server.ObjectType;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace attendance_server.Controllers
{
    public class authenticationController : Controller
    {
        private readonly FacultyDbContext _context;

        public authenticationController(FacultyDbContext context, IConfiguration configuration)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser(SignupBody body)
        {
            if (await _context.Faculty.AnyAsync(x => x.email == body.email))
            {

                return Ok("User Already Exist!");
            }
            var faculty = new FacultyModel
            {
                email = body.email,
                pass = BCrypt.Net.BCrypt.EnhancedHashPassword(body.password),
            };

            _context.Faculty.Add(faculty);
            await _context.SaveChangesAsync();

    


            return Ok("Register Successfuly");
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody]SigninBody body)
        {
            var faculty = await _context.Faculty.FirstOrDefaultAsync(u => u.email == body.email);

            var passwordVerified = BCrypt.Net.BCrypt.EnhancedVerify( body.password, faculty.pass);

            if (faculty == null || !passwordVerified)
            {
                
                return BadRequest("Invalid Credentials");
            }

            var token = GenerateToken(faculty.email);


            return Ok(token);
        }

        public static string GenerateToken(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("absbsbsbsbshdjdhdjdjjjdgsdgshgdshgdshdgh"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
 {
     new Claim(JwtRegisteredClaimNames.Email, email),
     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
 };



            var token = new JwtSecurityToken(
                issuer: "http://localhost:5099",
                audience: "http://localhost:5099",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    }
