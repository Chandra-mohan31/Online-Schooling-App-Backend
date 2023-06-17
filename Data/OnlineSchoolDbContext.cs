using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Data
{
    public class OnlineSchoolDbContext:IdentityDbContext<OnlineSchoolUser>
    {
        public OnlineSchoolDbContext(DbContextOptions<OnlineSchoolDbContext> options) : base(options) { } 
        
        public DbSet<OnlineSchoolUser> OnlineSchoolUsers { get; set; }  
    }
}
