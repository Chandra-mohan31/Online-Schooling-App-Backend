using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class TeacherSubjects
    {
      

     

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public OnlineSchoolUser Teacher { get; set; }

        public SchoolSubjects Subject { get; set; }

        //public TeacherSubjects(OnlineSchoolUser Teacher, SchoolSubjects Subject)
        //{
        //    this.Teacher = Teacher;
        //    this.Subject = Subject;
        //}
    }
}
