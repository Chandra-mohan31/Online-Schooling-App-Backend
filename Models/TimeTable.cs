using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class TimeTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        public String Day { get; set; }

        public SchoolClasses Class { get; set; }

        public ClassHours Hour { get; set; }

        public TeacherSubjects HandlingStaff { get; set; }

        public String MeetLink { get; set; }



    }
}
