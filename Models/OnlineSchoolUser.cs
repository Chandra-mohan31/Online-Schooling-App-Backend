using Microsoft.AspNetCore.Identity;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class OnlineSchoolUser: IdentityUser
    {
        
        public DateTime? Dob { get; set; }


        public String? ImageUrl { get; set; } 

        public String? EmailConfirmationToken { get; set; }
    }
}
