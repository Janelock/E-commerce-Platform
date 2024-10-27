using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Helpers;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;


        public RegistrationController(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostMe>>> HelloJoe(){
            return await _context.PostMe.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        [Route("signOut")]
        public async Task<IActionResult> SignOut()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid user.");
            }

            var user = await _context.Users
                                    .Where(u => u.Email == email)
                                    .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Error");
            }

            if (!user.IsActive)
            {
                return BadRequest("User was not signed in.");
            }

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok("User Signed Out Successfully.");
        }

        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(User user)
        {
            // Check if the email already exists
            var existingUser = await _context.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            
            if(existingUser != null)
                return BadRequest("Email already in use");
            
             // Create the new user
            var newUser = new User
            {
                Email = user.Email,
                Password = Helpers.Helpers.HashPassword(user.Password),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                IsActive = true,
            };

            _context.Users.Add(newUser);

            try
            {
                await _context.SaveChangesAsync();
                var token = GenerateJwtToken(newUser.Email);
                return Ok(token);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, "Error registering user");
            }
        }
        
        [HttpPost]
        [Route("signIn")]
        public async Task<IActionResult> SignIn(SignInRequest signInRequest)
        {
            var user = await _context.Users
                                    .Where(u => u.Email == signInRequest.Email)
                                    .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }

            if (user.IsActive)
            {
                return BadRequest("Already Signed In");
            }

            bool isPasswordValid = Helpers.Helpers.VerifyPassword(user.Password, signInRequest.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Invalid email or password.");
            }

            user.IsActive = true;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user.Email);
            return Ok(token.ToString());
        }
        
        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                IssuedAt = DateTime.UtcNow,
                //Expires = DateTime.UtcNow.AddMinutes(30),
                Expires = DateTime.UtcNow.AddDays(1),
                
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}