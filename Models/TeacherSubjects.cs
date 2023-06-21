namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class TeacherSubjects
    {
        public int Id { get; set; }

        public OnlineSchoolUser Teacher { get; set; }

        public SchoolSubjects Subject { get; set; }    
    }
}
