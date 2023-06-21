using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class StudentClasses
    {


      

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public OnlineSchoolUser Student { get; set; }

        public SchoolClasses Class { get; set; }
        //public StudentClasses(OnlineSchoolUser Student, SchoolClasses Class)
        //{
        //    this.Student = Student;
        //    this.Class = Class;
        //}
    }
}
