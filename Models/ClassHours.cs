using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class ClassHours
    {
        [Key]
        public String Session { get; set; }

        public String Timing { get; set; }


    }
}
