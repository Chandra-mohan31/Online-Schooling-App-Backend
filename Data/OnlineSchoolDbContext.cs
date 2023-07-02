using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Data
{
    public class OnlineSchoolDbContext:IdentityDbContext<OnlineSchoolUser>
    {
        public OnlineSchoolDbContext(DbContextOptions<OnlineSchoolDbContext> options) : base(options) { } 
        
        public DbSet<OnlineSchoolUser> OnlineSchoolUsers { get; set; }
        public DbSet<SchoolClasses> classesAvailable { get; set; }
        public DbSet<SchoolSubjects> subjectsAvailable { get; set; }
        public DbSet<TeacherSubjects> TeacherSubjects { get; set; }

        public DbSet<StudentClasses> StudentClasses { get; set; }

        public DbSet<AttendanceModel> Attendance { get; set; }

        public DbSet<ClassHours> ClassHoursTable { get; set; }


        public DbSet<TimeTable> TimeTable { get; set; }


        public DbSet<ClassMaterials> ClassStudyMaterials { get; set; }

    }
}
