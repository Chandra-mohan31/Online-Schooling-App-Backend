using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<OnlineSchoolUser> _userManager;
        private readonly OnlineSchoolDbContext _context;

        public UserManagerController(UserManager<OnlineSchoolUser> userManager, OnlineSchoolDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {


            var users = _userManager.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber,
                u.Dob,
                u.ImageUrl,
                u.EmailConfirmed
            });
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetailsFromId([FromRoute] String userId)
        {
            Console.WriteLine(userId);
            Console.WriteLine("got the user id");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPut]
        [Route("users/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UserUpdateModel model)
        {
            // Check if the user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            //TODO : verify email while updating details 
            // Update the user details
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.Dob = model.Dob;
            user.ImageUrl = model.ImageUrl;

            // Add more properties to update as needed

            // Save the changes to the database
            _context.SaveChanges();

            return Ok(new { message = "user details updated!" });
        }
    }
    }
