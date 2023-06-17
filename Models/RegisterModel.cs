using System.ComponentModel.DataAnnotations;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class RegisterModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
