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


        public String? ImageUrl { get; set; }

        public String? UserName { get; set; }

        public String? PhoneNumber { get; set; }

        public DateTime? dob { get; set; }

        public String UserRole { get; set; }

        public String? Subject { get; set; }

        public String? Class { get; set; }
    }
}
