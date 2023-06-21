namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class StudentClasses
    {
        public int Id { get; set; }

        public OnlineSchoolUser Student { get; set; }

        public SchoolClasses Class { get; set; }
    }
}
