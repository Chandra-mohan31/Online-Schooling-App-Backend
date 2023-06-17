using Microsoft.AspNetCore.Identity;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class OnlineSchoolUser: IdentityUser
    {
        
        public DateTime? Dob { get; set; }
    }
}
