using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using ONLINE_SCHOOL_BACKEND.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static System.Net.WebRequestMethods;

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


        private void sendConfirmationEmail(String confirmationLink,String toEmail)
        {
            toEmail = "testname1234554321@gmail.com";//for testing purposes
            try
            {

                string fromPassword = _configuration["fromEmailPass"];
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("testname1234554321@gmail.com"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Confirm your Account for OnlineSchool";
                string text = $"<div><h1>Online School</h1><a href=\"{confirmationLink}\">Click here to Confirm!</a></div>";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = text,
                    
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate("testname1234554321@gmail.com", fromPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new OnlineSchoolUser { UserName = model.UserName, Email = model.Email,PhoneNumber = model.PhoneNumber,Dob = model.dob,ImageUrl = model.ImageUrl };
                var result = await _userManager.CreateAsync(user, model.Password);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                Console.WriteLine(encodedToken);
                var confirmationLink = "https://localhost:7274/api/Authentication/confirm-email?userId=" + user.Id + "&token="+encodedToken;
           


                Console.WriteLine(confirmationLink);
                Console.WriteLine("confirmation link generated!");
                if (result.Succeeded)
                {

                    sendConfirmationEmail(confirmationLink,user.Email);
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
                    if (user.EmailConfirmed)
                    {
                        var token = GenerateJwtToken(user);

                        return Ok(new { AccessToken = token, message = "Logged In successfully!" });
                    }
                    else
                    {
                        return Ok(new { message = "Please confirm your Email!" });
                    }
                  
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

        [HttpGet("/logout")] 
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Signed Out successfully!");
        }



        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            Console.WriteLine(userId);
            Console.WriteLine(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            Console.WriteLine(result);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Email confirmation failed.");
        }


    }
}
