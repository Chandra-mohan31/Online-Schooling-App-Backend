using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ONLINE_SCHOOL_BACKEND.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<OnlineSchoolUser> _userManager;
        private readonly SignInManager<OnlineSchoolUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<OnlineSchoolUser> userManager, SignInManager<OnlineSchoolUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new OnlineSchoolUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                  

                    return Ok("Registered successfully.");
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(errors);
                }
            }

            return BadRequest(ModelState);
        }


        private string GenerateJwtToken(OnlineSchoolUser user)
        {
            Console.WriteLine(_configuration["JwtSecret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSecret"]);
            Console.WriteLine(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
                // Add additional claims as needed
            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


       
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user);

                    return Ok(new { AccessToken = token,message="Logged In successfully!" });
                }
            }

            return BadRequest(ModelState);
        }


        [HttpGet("user-details")]
        public async Task<IActionResult> GetUserDetailsFromToken(string accessToken)
        {




            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(accessToken);
            Console.WriteLine("decoded token");
            string userId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;

            Console.WriteLine("User ID: " + userId);
            OnlineSchoolUser user = await _userManager.FindByIdAsync(userId);

            Console.WriteLine("got user details");
            if (decodedToken.Claims.Any())
            {
                return Ok(user);
            }

            return BadRequest("Invaild Access Token");


        }



    }
}
