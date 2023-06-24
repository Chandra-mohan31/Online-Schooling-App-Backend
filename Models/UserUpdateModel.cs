namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class UserUpdateModel
    {
        public String? UserName;

        public String? Email;

        public String? PhoneNumber;
        public DateTime? Dob { get; set; }

        public String? ImageUrl { get; set; }
        

    }
}
