using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class ClassMaterials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public OnlineSchoolUser PostedBy { get; set; }  

        public SchoolSubjects Subject { get; set; }

        public SchoolClasses ForClass { get; set; }

        public string MaterialTitle { get; set; }

        public string Description { get; set; }

        public string MaterialContentUrl { get; set; }

        public DateTime PostedOn { get; set; }

        public string MaterialContentType { get; set; }
    }
}
