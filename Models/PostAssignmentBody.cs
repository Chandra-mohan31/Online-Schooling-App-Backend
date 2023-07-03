namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class PostAssignmentBody
    {

        public string ForClass { get; set; }

        public string PostedByEmail { get; set; }

        public string SubjectName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDateTime { get; set; }
    }
}
