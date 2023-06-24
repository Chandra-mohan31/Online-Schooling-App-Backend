using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class AttendanceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public OnlineSchoolUser User { get; set; }

        public DateTime Date { get; set; }

        public String hour { get; set; }

        public Boolean present { get; set; }


    }
}
