using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class AssignmentModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public SchoolClasses ForClass { get; set; }

        public OnlineSchoolUser PostedBy { get; set; }

        public SchoolSubjects Subject { get; set; }

        public String Title { get; set; }

        public String Description { get; set; } 

        public DateTime DueDateTime { get; set; }
    }
}
