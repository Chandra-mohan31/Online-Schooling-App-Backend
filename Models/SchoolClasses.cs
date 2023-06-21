using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class SchoolClasses
    {
        [Key]
        public String ClassName { get; set; }
    }
}
