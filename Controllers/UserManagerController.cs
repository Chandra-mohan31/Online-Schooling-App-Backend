using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<OnlineSchoolUser> _userManager;
        public UserManagerController(UserManager<OnlineSchoolUser> userManager) { 
            _userManager = userManager;
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
        public async Task<IActionResult> GetUserDetailsFromId([FromRoute]String userId)
        {
            Console.WriteLine(userId);
            Console.WriteLine("got the user id");
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUserDetails([FromRoute] string userId, UserUpdateModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            Console.WriteLine(model.Dob.ToString());
            Console.WriteLine(model.Dob.ToString());

            // Update user details based on the model
            user.UserName = model.UserName != null ? model.UserName : user.UserName;
            user.Email = model.Email != null ? model.Email : user.Email;
            user.PhoneNumber = model.PhoneNumber != null ? model.PhoneNumber : user.PhoneNumber;
            user.Dob = model.Dob != null ? model.Dob : user.Dob;
            user.ImageUrl = model.ImageUrl != null ? model.ImageUrl : user.ImageUrl;

            // Save the changes
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("User details updated successfully.");
            }

            // If the update fails, return the errors
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(errors);
        }

    }
}
