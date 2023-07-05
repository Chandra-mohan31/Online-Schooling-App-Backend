namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class AssignmentSubmissionBody
    {

        public int AssignmentId { get; set; }

        public String StudentUserName { get; set; }


        public String StudentSubmissionFileURL { get; set; }


        public String StudentProfileUrl { get; set; }


        public String FileName { get; set; }

        public String FileType { get; set; }


    }
}
