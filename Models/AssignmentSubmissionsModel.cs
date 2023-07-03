using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class AssignmentSubmissionsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        public AssignmentModel Assignment { get; set; }

        public String StudentUserName { get; set; }


        public String StudentSubmissionFileURL { get; set; }

        public String Status { get; set; }

        public DateTime SubmissionDateTime { get; set; }

    }
}
